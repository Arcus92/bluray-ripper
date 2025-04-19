using BluRayLib.FFmpeg;
using BluRayLib.Ripper.Output;
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
    /// <param name="output">The output definition.</param>
    /// <param name="onUpdate">The status update event.</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task ExportAsync(string outputPath, OutputInfo output, Action<ConverterUpdate>? onUpdate = null,
        CancellationToken cancellationToken = default)
    {
        if (output.Source.Type != OutputSourceType.BluRay)
        {
            throw new ArgumentException("Output source is not from BluRay!", nameof(output));
        }
        if (output.Source.DiskName != _bluRay.DiskName)
        {
            throw new ArgumentException("Output source disk name mismatch!", nameof(output));
        }
        if (output.Source.Segments.Length == 0)
        {
            throw new ArgumentException("Output source has no segments!", nameof(output));
        }
        if (!_bluRay.Playlists.TryGetValue(output.Source.PlaylistId, out var playlist))
        {
            throw new ArgumentException("Couldn't load playlist from disk!", nameof(output));
        }
        var firstSegment = output.Source.Segments[0];
        
        
        var ffmpeg = new Engine();
        
        _logger.LogInformation("Collecting metadata from segment {SegmentId:00000} for playlist {PlaylistId:00000}", 
            firstSegment.Id, output.Source.PlaylistId);
        
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
        foreach (var segment in output.Source.Segments)
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
            output.Source.PlaylistId, outputPath, output.Name); 
        
        // Convert the file
        var renameMap = new Dictionary<string, string>();
        await ffmpeg.ConvertAsync(builder =>
        {
            // Builds the concat text file in memory
            var concatStream = new MemoryStream();
            var concatWriter = new StreamWriter(concatStream);
            foreach (var segment in output.Source.Segments)
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
            
            if (output.ExportChapters)
            {
                // Builds the chapter file in memory
                var chapterStream = new MemoryStream();
                var chapterWriter = new StreamWriter(chapterStream);
                foreach (var chapter in output.Chapters)
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

            // FFmpeg supports multiple outputs. We can export the subtitle files in a single run as well.
            // We just need to create a new mapping and then define a new output.
            foreach (var file in output.Files)
            {
                // Define codec
                builder.Codec(StreamType.Video, output.Codec.VideoCodec);
                builder.Codec(StreamType.Audio, output.Codec.AudioCodec);
                builder.Codec(StreamType.Subtitle, output.Codec.SubtitleCodec);
            
                if (output.Codec.ConstantRateFactor.HasValue) builder.ConstantRateFactor(output.Codec.ConstantRateFactor.Value);
                if (output.Codec.MaxRate.HasValue) builder.MaxRate(output.Codec.MaxRate.Value);
                if (output.Codec.BufferSize.HasValue) builder.BufferSize(output.Codec.BufferSize.Value);
                
                // Map the output streams
                var outputStreamCount = 0;
                foreach (var stream in file.Streams)
                {
                    if (!stream.Enabled) continue;
                    builder.Map(input, pidToIndex[stream.Id]);
                    if (!string.IsNullOrEmpty(stream.LanguageCode))
                        builder.Metadata(outputStreamCount, "language", stream.LanguageCode);
                    if (stream.Default)
                        builder.Disposition(outputStreamCount, "default");
                    outputStreamCount++;
                }
                
                // Video output
                builder.OverwriteOutput();
                builder.Format(file.Format);

                var filename = file.Filename;
                
                // Add working file extension 
                if (WorkingFileExtension is not null)
                {
                    var newFilename = $"{filename}{WorkingFileExtension}";
                    renameMap.Add(filename, newFilename);
                    filename = newFilename;
                }
                
                var path = Path.Combine(outputPath, filename);
                builder.Output(path);
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
            output.Source.PlaylistId, outputPath); 
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