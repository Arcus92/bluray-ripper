using MediaLib.FFmpeg;
using MediaLib.Output;
using Microsoft.Extensions.Logging;

namespace MediaLib.Providers;

/// <summary>
/// A basic converter to convert a media file with <see cref="FFmpeg"/>.
/// </summary>
/// <typeparam name="TProvider">The provider.</typeparam>
public abstract class FFmpegMediaConverter<TProvider> : IMediaConverter where TProvider : IMediaProvider
{
    protected readonly TProvider Provider;
    protected readonly ILogger Logger;
    protected readonly MediaConverterParameter Parameter;
    
    public FFmpegMediaConverter(ILogger logger, TProvider provider, MediaConverterParameter parameter)
    {
        Logger = logger;
        Provider = provider;
        Parameter = parameter;
    }
    
    /// <summary>
    /// Gets and sets the file extension added to the output files while the export is running.
    /// </summary>
    public string? WorkingFileExtension { get; set; } = ".tmp";
    
    /// <summary>
    /// The rename map to store the original and the temporary name.
    /// </summary>
    private readonly Dictionary<string, string> _renameMap = new();

    /// <summary>
    /// Must be called before using <see cref="GetWorkingFilename"/>.
    /// </summary>
    private void InitWorkingFilenames()
    {
        _renameMap.Clear();
    }

    /// <summary>
    /// Returns a filename with the <see cref="WorkingFileExtension"/>.
    /// </summary>
    /// <param name="originalFilename">The original filename.</param>
    /// <returns>Returns a working filename.</returns>
    private string GetWorkingFilename(string originalFilename)
    {
        if (WorkingFileExtension is null) return originalFilename;
        
        // Add working file extension 
        var newFilename = $"{originalFilename}{WorkingFileExtension}";
        _renameMap.Add(originalFilename, newFilename);
        return newFilename;
    }

    /// <summary>
    /// Must be called after all files were converted. This will remove the working extension from all files.
    /// </summary>
    /// <param name="outputPath">The output path to apply the renaming.</param>
    private void ApplyWorkingFilenames(string outputPath)
    {
        // Rename working files
        foreach (var (filename, workingFilename) in _renameMap)
        {
            var path = Path.Combine(outputPath, filename);
            var workingPath = Path.Combine(outputPath, workingFilename);
            File.Move(workingPath, path);
        }
    }

    /// <summary>
    /// Returns the index of the FFmpeg stream used for mapping the source data.
    /// This should either be <see cref="StreamMetadata.Id"/> or <see cref="StreamMetadata.Pid"/>.
    /// </summary>
    protected virtual ulong GetStreamIndex(StreamMetadata stream) => stream.Id;
    
    /// <summary>
    /// Handles the opening of a segment.
    /// </summary>
    /// <param name="segmentId">The segment id.</param>
    /// <returns>Returns the segment stream.</returns>
    protected abstract Stream OpenSegmentStream(ushort segmentId);
    
    /// <summary>
    /// Returns the raw filesize of the segment.
    /// </summary>
    /// <param name="segmentId">The segment id.</param>
    /// <returns>Returns the segment filesize.</returns>
    protected abstract long GetSegmentFilesize(ushort segmentId);

    /// <inheritdoc />
    public virtual async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var definition = Parameter.Definition;
        var outputPath = Parameter.Path;
        var segmentIds = Parameter.Definition.Identifier.SegmentIds;
        var firstSegmentId = segmentIds.First();
        var onUpdate = Parameter.OnUpdate;
        
        if (!Provider.Contains(Parameter.Definition.Identifier))
        {
            throw new ArgumentException("Output source is not defined by provider!", nameof(definition));
        }

        var ffmpeg = new Engine();
        
        // Mapping the pid to the FFmpeg index
        var idToStream = new Dictionary<ulong, StreamMetadata>();
        Logger.LogInformation("Collecting metadata from segment {SegmentId} for {Id}",
            firstSegmentId, definition.Identifier.Id);

        // Before converting, we need to fetch the internal FFmpeg stream index. We cannot use the PIDs for that, and 
        // the order of stream may differ ot hidden streams change the order.
        var metadata = await ffmpeg.GetMetadataAsync(builder =>
        {
            var inputStream = builder.CreateInputStream(() => OpenSegmentStream(firstSegmentId));
            builder.Input(inputStream);
        }, cancellationToken);

        
        foreach (var input in metadata)
        {
            foreach (var stream in input.Streams)
            {
                // I had multiple audio entries with the same PID. Using the last one worked for me.
                idToStream[GetStreamIndex(stream)] = stream;
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
            totalInputSize += GetSegmentFilesize(segmentId);
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
        
        Logger.LogInformation("Starting export of {Id} to {OutputPath} as {Basename}", 
            definition.Identifier.Id, outputPath, definition.MediaInfo.Name); 

        // Convert the file
        InitWorkingFilenames();
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
                    
                    if (!idToStream.TryGetValue(stream.Id, out var ffmpegStream))
                    {
                        Logger.LogError("Couldn't find stream {StreamId} in source file.", stream.Id); 
                        continue;
                    }

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

                var filename = GetWorkingFilename(file.Filename);
                
                var path = Path.Combine(outputPath, filename);
                builder.Output(path);
            }

        }, newOnUpdate, cancellationToken);

        ApplyWorkingFilenames(outputPath);
        
        Logger.LogInformation("Stream {Id} was exported to {OutputPath}", 
            definition.Identifier.Id, outputPath); 
    }
}