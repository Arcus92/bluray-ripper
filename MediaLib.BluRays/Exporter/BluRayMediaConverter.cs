using MediaLib.BluRays.Providers;
using MediaLib.FFmpeg;
using MediaLib.Output;
using MediaLib.Providers;
using Microsoft.Extensions.Logging;

namespace MediaLib.BluRays.Exporter;

public class BluRayMediaConverter : IMediaConverter
{
    private readonly ILogger _logger;
    private readonly BluRayMediaProvider _provider;
    private readonly MediaConverterParameter _parameter;
    
    public BluRayMediaConverter(ILogger logger, BluRayMediaProvider provider, MediaConverterParameter parameter)
    {
        _logger = logger;
        _provider = provider;
        _parameter = parameter;
    }

    /// <summary>
    /// Gets and sets the file extension added to the output files while the export is running.
    /// </summary>
    public string? WorkingFileExtension { get; set; } = ".tmp";
    
    /// <inheritdoc />
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var definition = _parameter.Definition;
        var outputPath = _parameter.Path;
        var onUpdate = _parameter.OnUpdate;
        
        if (!_provider.Contains(_parameter.Definition.Identifier))
        {
            throw new ArgumentException("Output source is not from BluRay!", nameof(definition));
        }
        if (!_provider.BluRay.Playlists.TryGetValue(definition.Identifier.Id, out var playlist))
        {
            throw new ArgumentException("Couldn't load playlist from disk!", nameof(definition));
        }
        var segmentIds = playlist.Items.Select(i => ushort.Parse(i.Name)).ToArray();
        var firstSegmentId = segmentIds.First();
        
        var ffmpeg = new Engine();
        
        _logger.LogInformation("Collecting metadata from segment {SegmentId:00000} for playlist {PlaylistId:00000}", 
            firstSegmentId, definition.Identifier.Id);
        
        // Before converting, we need to fetch the internal FFmpeg stream index. We cannot use the PIDs for that, and 
        // the order of stream may differ ot hidden streams change the order.
        var metadata = await ffmpeg.GetMetadataAsync(builder =>
        {
            var inputStream = builder.CreateInputStream(() => OpenSegmentStream(firstSegmentId));
            builder.Input(inputStream);
        }, cancellationToken);

        // Mapping the pid to the FFmpeg index
        var pidToStream = new Dictionary<ushort, StreamMetadata>();
        foreach (var input in metadata)
        {
            foreach (var stream in input.Streams)
            {
                // I had multiple audio entries with the same PID. Using the last one worked for me.
                pidToStream[stream.Pid] = stream;
            }
        }
        
        // Collecting total input filesize. The FFmpeg time code doesn't work for progress tracking.
        // It will only show the time code for the last stream in our output. This is almost always a subtitle. To be
        // exact, a forced subtitle that is only used a few times in the video.
        // To have a better progress status, we'll track the file position of the virtual input streams.
        long totalInputSize = 0;
        var inputStreams = new List<InputStream>();
        foreach (var segmentId in segmentIds)
        {
            var fileInfo = _provider.BluRay.GetM2TsFileInfo(segmentId);
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
            definition.Identifier.Id, outputPath, definition.MediaInfo.Name); 
        
        // Convert the file
        var renameMap = new Dictionary<string, string>();
        await ffmpeg.ConvertAsync(builder =>
        {
            // Builds the concat text file in memory
            var concatStream = new MemoryStream();
            var concatWriter = new StreamWriter(concatStream);
            foreach (var segmentId in segmentIds)
            {
                var inputStream = builder.CreateInputStream(() => OpenSegmentStream(segmentId));
                concatWriter.WriteLine($"file '{inputStream.GetPath()}'");
                inputStreams.Add(inputStream);
            }
            concatWriter.Flush();
            concatStream.Position = 0;

            builder.Format("concat");
            builder.Safe(0);
            var input = builder.Input(concatStream);
            
            if (definition.ExportChapters)
            {
                // Builds the chapter file in memory
                var chapterStream = new MemoryStream();
                var chapterWriter = new StreamWriter(chapterStream);
                foreach (var chapter in definition.Chapters)
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
            foreach (var file in definition.Files)
            {
                // Define codec
                builder.Codec(StreamType.Video, definition.Codec.VideoCodec);
                builder.Codec(StreamType.Audio, definition.Codec.AudioCodec);
                builder.Codec(StreamType.Subtitle, definition.Codec.SubtitleCodec);
            
                if (definition.Codec.ConstantRateFactor.HasValue) builder.ConstantRateFactor(definition.Codec.ConstantRateFactor.Value);
                if (definition.Codec.MaxRate.HasValue) builder.MaxRate(definition.Codec.MaxRate.Value);
                if (definition.Codec.BufferSize.HasValue) builder.BufferSize(definition.Codec.BufferSize.Value);
                
                // Map the output streams
                var outputStreamCount = 0;
                foreach (var stream in file.Streams)
                {
                    if (!stream.Enabled) continue;

                    if (!pidToStream.TryGetValue(stream.Id, out var ffmpegStream)) continue;
                    
                    builder.Map(input, (int)ffmpegStream.Id);
                    if (!string.IsNullOrEmpty(stream.LanguageCode))
                        builder.Metadata(outputStreamCount, "language", stream.LanguageCode);
                    if (stream.Default)
                        builder.Disposition(outputStreamCount, "default");
                    
                    // BluRay PCM isn't supported outside M2TS and must be changed to regular PCM.
                    if (stream.Type == OutputStreamType.Audio &&
                        ffmpegStream.Format.StartsWith("pcm_bluray") &&
                        definition.Codec.AudioCodec == "copy")
                    {
                        builder.Codec(outputStreamCount, "pcm_s24le");
                    }
                    
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
            definition.Identifier.Id, outputPath); 
    }
    
    /// <summary>
    /// Handles the opening of a segment.
    /// </summary>
    /// <param name="clipId">The segment id.</param>
    /// <returns></returns>
    private Stream OpenSegmentStream(ushort clipId)
    {
        var retries = 0;
        const int maxRetries = 5;
        while (true)
        {
            try
            {
                _logger.LogInformation("Opening segment {SegmentId:00000}.m2ts", clipId);
                return _provider.BluRay.GetM2TsStream(clipId);
            }
            catch (Exception ex)
            {
                if (retries < maxRetries)
                {
                    retries++;
                    _logger.LogWarning(ex, "Exception while opening segment {SegmentId:00000}.m2ts. Retry {Retry} / {MaxRetry}", clipId, retries, maxRetries);
                }
                else
                {
                    _logger.LogError(ex, "Exception while opening segment {SegmentId:00000}.m2ts!", clipId);
                    throw;
                }
            }
        }
    }
}