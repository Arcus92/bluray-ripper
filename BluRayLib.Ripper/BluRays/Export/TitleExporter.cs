using BluRayLib.FFmpeg;
using Microsoft.Extensions.Logging;

namespace BluRayLib.Ripper.BluRays.Export;

/// <summary>
/// This manages the title export from BluRay disks.
/// </summary>
public class TitleExporter
{
    private readonly ILogger _logger;
    private readonly BluRay _bluRay;

    public TitleExporter(ILogger logger, BluRay bluRay)
    {
        _logger = logger;
        _bluRay = bluRay;
    }

    /// <summary>
    /// Gets and sets the file extension added to the output files while the export is running.
    /// </summary>
    public string? WorkingFileExtension { get; set; } = ".tmp";

    /// <summary>
    /// Exports the playlist as a video file using FFmpeg.
    /// </summary>
    /// <param name="outputPath">The output path.</param>
    /// <param name="options">The title export options.</param>
    /// <param name="onUpdate">The status update event.</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task ExportAsync(string outputPath, TitleExportOptions options, Action<ConverterUpdate>? onUpdate = null,
        CancellationToken cancellationToken = default)
    {
        if (options.Title.Segments.Length == 0)
        {
            _logger.LogError("Cannot export playlist {PlaylistId:00000} with no segments.", options.Title.Id);
            return;
        }
        var firstSegment = options.Title.Segments.First();
        
        
        var ffmpeg = new Engine();
        
        _logger.LogInformation("Collecting metadata from segment {SegmentId:00000} for playlist {PlaylistId:00000}", 
            firstSegment.Id, options.Title.Id);
        
        // Before converting, we need to fetch the internal FFmpeg stream index. We cannot use the PIDs for that, and 
        // the order of stream may differ ot hidden streams change the order.
        var metadata = await ffmpeg.GetMetadataAsync(builder =>
        {
            var inputStream = builder.CreateInputStream(() => OpenSegmentStream(firstSegment.Id));
            builder.Input(inputStream);
        }, cancellationToken);

        // Mapping the pid to the FFmpeg index
        var pidToIndex = new Dictionary<ushort, int>();
        foreach (var input in metadata)
        {
            foreach (var stream in input.Streams)
            {
                // I had multiple audio entries with the same PID. Using the last one worked for me.
                pidToIndex[stream.Pid] = (int)stream.Id;
            }
        }
        
        // Collecting total input filesize. The FFmpeg time code doesn't work for progress tracking.
        // It will only show the time code for the last stream in our output. This is almost always a subtitle. To be
        // exact, a forced subtitle that is only used a few times in the video.
        // To have a better progress status, we'll track the file position of the virtual input streams.
        long totalInputSize = 0;
        var inputStreams = new List<InputStream>();
        foreach (var segment in options.Title.Segments)
        {
            var fileInfo = _bluRay.GetM2TsFileInfo(segment.Id);
            totalInputSize += fileInfo.Length;
        }
        
        // Build a better update event to calculate the percentage value by consumed bytes.
        Action<ConverterUpdate>? newOnUpdate = null;
        if (onUpdate is not null)
        {
            newOnUpdate = update =>
            {
                long position = 0;
                foreach (var inputStream in inputStreams)
                {
                    position += inputStream.Position;
                }

                update.Percentage = position / (double)totalInputSize;
                onUpdate(update);
            };
        }
        
        _logger.LogInformation("Starting export of playlist {PlaylistId:00000} to {OutputPath} as {Basename}", 
            options.Title.Id, outputPath, options.Basename); 
        
        // Convert the file
        var renameMap = new Dictionary<string, string>();
        await ffmpeg.ConvertAsync(builder =>
        {
            // Builds the concat text file in memory
            var concatStream = new MemoryStream();
            var concatWriter = new StreamWriter(concatStream);
            foreach (var segment in options.Title.Segments)
            {
                var inputStream = builder.CreateInputStream(() => OpenSegmentStream(segment.Id));
                concatWriter.WriteLine($"file '{inputStream.GetPath()}'");
                inputStreams.Add(inputStream);
            }
            concatWriter.Flush();
            concatStream.Position = 0;

            builder.Format("concat");
            builder.Safe(0);
            var input = builder.Input(concatStream);
            
            if (options.ExportChapters)
            {
                // Builds the chapter file in memory
                var chapterStream = new MemoryStream();
                var chapterWriter = new StreamWriter(chapterStream);
                foreach (var chapter in options.Title.Chapters)
                {
                    var start = (ulong)(chapter.Start.TotalSeconds * 1000);
                    var end = (ulong)(chapter.End.TotalSeconds * 1000);
                    chapterWriter.WriteLine("[CHAPTER]");
                    chapterWriter.WriteLine("TIMEBASE=1/1000");
                    chapterWriter.WriteLine($"START={start}");
                    chapterWriter.WriteLine($"END={end}");
                    chapterWriter.WriteLine($"title={chapter.Name}");
                    chapterWriter.WriteLine();
                }

                chapterWriter.Flush();
                chapterStream.Position = 0;

                // Map the chapter
                builder.Format("ffmetadata");
                var inputChapter = builder.Input(chapterStream);
                builder.MapChapters(inputChapter);
            }

            // Define codec
            builder.Codec(StreamType.Video, options.Codec.VideoCodec);
            builder.Codec(StreamType.Audio, options.Codec.AudioCodec);
            builder.Codec(StreamType.Subtitle, options.Codec.SubtitleCodec);
            
            if (options.Codec.ConstantRateFactor.HasValue) builder.ConstantRateFactor(options.Codec.ConstantRateFactor.Value);
            if (options.Codec.MaxRate.HasValue) builder.MaxRate(options.Codec.MaxRate.Value);
            if (options.Codec.BufferSize.HasValue) builder.BufferSize(options.Codec.BufferSize.Value);
            
            // Map the output streams
            var outputStreamCount = 0;
            foreach (var stream in firstSegment.VideoStreams)
            {
                if (options.IgnoredStreamIds?.Contains(stream.Id) ?? false) continue;
                builder.Map(input, pidToIndex[stream.Id]);
                outputStreamCount++;
            }
            foreach (var stream in firstSegment.AudioStreams)
            {
                if (options.IgnoredStreamIds?.Contains(stream.Id) ?? false) continue;
                builder.Map(input, pidToIndex[stream.Id]);
                if (!string.IsNullOrEmpty(stream.LanguageCode))
                    builder.Metadata(outputStreamCount, "language", stream.LanguageCode);
                if (options.DefaultAudioStreamId == stream.Id)
                    builder.Disposition(outputStreamCount, "default");

                outputStreamCount++;
            }
            if (!options.ExportSubtitlesAsSeparateFiles) // Only included if not exported as separate files...
            {
                foreach (var stream in firstSegment.SubtitleStreams)
                {
                    if (options.IgnoredStreamIds?.Contains(stream.Id) ?? false) continue;
                    builder.Map(input, pidToIndex[stream.Id]);
                    if (!string.IsNullOrEmpty(stream.LanguageCode))
                        builder.Metadata(outputStreamCount, "language", stream.LanguageCode);
                    if (options.DefaultSubtitleStreamId == stream.Id)
                        builder.Disposition(outputStreamCount, "default");
                    
                    outputStreamCount++;
                }
            }
            
            // Video output
            builder.OverwriteOutput();
            if (options.VideoFormat is not null)
                builder.Format(options.VideoFormat);
            
            // Overwrite external filenames
            if (options.NameMap is null || !options.NameMap.TryGetValue(0, out var filename))
            {
                filename = $"{options.Basename}{options.Extension}";
            }

            // Add working file extension 
            if (WorkingFileExtension is not null)
            {
                var newFilename = $"{filename}{WorkingFileExtension}";
                renameMap.Add(filename, newFilename);
                filename = newFilename;
            }

            var path = Path.Combine(outputPath, filename);
            builder.Output(path);
            
            
            // FFmpeg supports multiple outputs. We can export the subtitle files in a single run as well.
            // We just need to create a new mapping and then define a new output.
            if (options.ExportSubtitlesAsSeparateFiles)
            {
                // Export subtitle files
                foreach (var stream in firstSegment.SubtitleStreams)
                {
                    if (options.IgnoredStreamIds?.Contains(stream.Id) ?? false) continue;
                    
                    builder.Map(input, pidToIndex[stream.Id]);
                    builder.Metadata(0, "language", stream.LanguageCode);
                    // Single exports don't need a default flag.
                    
                    builder.Codec(StreamType.Subtitle, options.Codec.SubtitleCodec);

                    // Subtitle output
                    builder.OverwriteOutput();

                    // Overwrite external filenames
                    if (options.NameMap is null || !options.NameMap.TryGetValue(stream.Id, out filename))
                    {
                        filename = $"{options.Basename}.{stream.LanguageCode}.{stream.Id}.sup";
                    }
                    
                    // Add working file extension 
                    if (WorkingFileExtension is not null)
                    {
                        var newFilename = $"{filename}{WorkingFileExtension}";
                        renameMap.Add(filename, newFilename);
                        filename = newFilename;
                    }
                    
                    path = Path.Combine(outputPath, filename);
                    builder.Format("sup");
                    builder.Output(path);
                }
            }
        }, newOnUpdate, cancellationToken);

        // Rename working files
        foreach (var (filename, workingFilename) in renameMap)
        {
            var path = Path.Combine(outputPath, filename);
            var workingPath = Path.Combine(outputPath, workingFilename);
            File.Move(workingPath, path);
        }
        
        _logger.LogInformation("Playlist {PlaylistId:00000} was exported to {OutputPath}", 
            options.Title.Id, outputPath); 
    }

    /// <summary>
    /// Handles the opening of a segment.
    /// </summary>
    /// <param name="clipId">The segment id.</param>
    /// <returns></returns>
    private Stream OpenSegmentStream(ushort clipId)
    {
        try
        {
            _logger.LogInformation("Opening segment {SegmentId:00000}.m2ts", clipId);
            return _bluRay.GetM2TsStream(clipId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while opening segment {SegmentId:00000}.m2ts!", clipId);
            throw;
        }
    }
}