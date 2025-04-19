using System.Diagnostics;
using System.Globalization;
using BluRayLib.Utils.IO;

namespace BluRayLib.FFmpeg;

public class Engine
{
    public Engine(string? ffmpegPath = null)
    {
        Binary = ffmpegPath ?? "ffmpeg";
    }
    
    /// <summary>
    /// Gets the path to the ffmpeg binary.
    /// </summary>
    public string Binary { get; }

    #region Run
    
    private readonly struct StartInfo
    {
        /// <summary>
        /// Gets the FFMpeg arguments.
        /// </summary>
        public string Arguments { get; init; }
        
        /// <summary>
        /// Gets the list of all bond input streams.
        /// </summary>
        public IList<InputStream>? InputStreams { get; init; }
        
        /// <summary>
        /// Gets the update / progress callback.
        /// </summary>
        public Action<ConverterUpdate>? OnUpdate { get; init; }
    }
    
    private readonly struct Result
    {
        /// <summary>
        /// Gets the FFmpeg exit code.
        /// </summary>
        public int ExitCode { get; init; }
        
        /// <summary>
        /// Gets the metadata of the input files.
        /// </summary>
        public InputMetadata[] Inputs { get; init; }
    }
    
    private async Task<Result> ExecuteAsync(StartInfo startInfo, CancellationToken cancellationToken = default)
    {
        var process = new Process();
        process.StartInfo = new ProcessStartInfo()
        {
            FileName = Binary,
            Arguments = startInfo.Arguments,
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true
        };
        
        // Start the input streams
        if (startInfo.InputStreams is not null)
        {
            foreach (var inputStream in startInfo.InputStreams)
            {
                await inputStream.StartAsync();
            }
        }
        
        process.Start();

        var inputs = new List<InputMetadata>();
        var intentReader = new IntentTextReader(process.StandardError, 2);
        await foreach (var lineRoot in intentReader.ReadBlockAsync().WithCancellation(cancellationToken))
        {
            if (lineRoot.StartsWith("Input #"))
            {
                var input = await ReadInputMedaDataAsync(intentReader);
                inputs.Add(input);
            }

            // Frame update
            if (lineRoot.StartsWith("frame="))
            {
                // No need to check frame updates
                if (startInfo.OnUpdate is null) continue;

                var frameStart = 6;
                
                var frameEnd = lineRoot.IndexOf("fps=", frameStart, StringComparison.Ordinal);
                if (frameEnd < 0) continue;
                var frameText = lineRoot.Substring(frameStart, frameEnd - frameStart).Trim();
                long? frame = null;
                if (long.TryParse(frameText, out var value))
                {
                    frame = value;
                }
                
                var timeStart = lineRoot.IndexOf("time=", frameEnd, StringComparison.Ordinal);
                if (timeStart < 0) continue;
                timeStart += 5;
                
                var timeEnd = lineRoot.IndexOf("bitrate=", timeStart, StringComparison.Ordinal);
                if (timeEnd < 0) continue;
                var timeText = lineRoot.Substring(timeStart, timeEnd - timeStart).Trim();
                TimeSpan? currentTime = null;
                if (TimeSpan.TryParse(timeText, out var time))
                {
                    currentTime = time;
                }

                // We need one input to determine the total duration. 
                // Currently, this won't handle offsets or lengths.
                if (inputs.Count <= 0) continue;
                var duration = inputs.Max(i => i.Duration);
                var percentage = currentTime?.TotalSeconds / duration.TotalSeconds;
                var update = new ConverterUpdate()
                {
                    Inputs = inputs,
                    Duration = duration,
                    Current = currentTime,
                    Frame = frame,
                    Percentage = percentage
                };

                startInfo.OnUpdate(update);
            }
        }
        
        try
        {
            await process.WaitForExitAsync(cancellationToken);
            return new Result()
            {
                ExitCode = process.ExitCode,
                Inputs = inputs.ToArray(),
            };
        }
        finally
        {
            // Close the input streams.
            if (startInfo.InputStreams is not null)
            {
                foreach (var inputStream in startInfo.InputStreams)
                {
                    inputStream.Dispose();
                }
            }
        }
    }
    
    #endregion Run
    
    #region Convert

    /// <summary>
    /// Runs ffmpeg with the given arguments.
    /// </summary>
    /// <param name="builderCallback">The argument builder callback.</param>
    /// <param name="onUpdate">The status update event.</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task ConvertAsync(Action<CommandBuilder> builderCallback, Action<ConverterUpdate>? onUpdate = null,
        CancellationToken cancellationToken = default)
    {
        var builder = new CommandBuilder();
        builderCallback(builder);
        await ConvertAsync(builder.Arguments, builder.InputStreams, onUpdate, cancellationToken);
    }

    /// <summary>
    /// Runs ffmpeg with the given arguments.
    /// </summary>
    /// <param name="arguments">The ffmpeg arguments.</param>
    /// <param name="inputStreams">The list of input streams.</param>
    /// <param name="onUpdate">The status update event.</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task ConvertAsync(string arguments, IList<InputStream>? inputStreams = null, Action<ConverterUpdate>? onUpdate = null,
        CancellationToken cancellationToken = default)
    {
        var startInfo = new StartInfo()
        {
            Arguments = arguments,
            InputStreams = inputStreams,
            OnUpdate = onUpdate,
        };
        var result = await ExecuteAsync(startInfo, cancellationToken);
        if (result.ExitCode != 0)
        {
            throw new IOException($"FFmpeg returned {result.ExitCode}!");
        }
    }
    
    #endregion Convert
    
    #region Probe

    /// <summary>
    /// Returns the metadata of the given file using ffmpeg.
    /// </summary>
    /// <param name="path">The input file.</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    public async Task<InputMetadata> GetMetadataAsync(string path, CancellationToken cancellationToken = default)
    {
        var inputs = await GetMetadataAsync(builder =>
        {
            builder.Input(path);
        }, cancellationToken);

        if (inputs.Length != 1)
        {
            throw new IOException($"FFmpeg returned wrong number of inputs! Should be one, but is {inputs.Length}.");
        }
        
        return inputs[0];
    }

    /// <summary>
    /// Returns the metadata of the given file using ffmpeg.
    /// </summary>
    /// <param name="builderCallback">The argument builder callback.</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    public async Task<InputMetadata[]> GetMetadataAsync(Action<CommandBuilder> builderCallback,
        CancellationToken cancellationToken = default)
    {
        var builder = new CommandBuilder();
        builderCallback(builder);
        return await GetMetadataAsync(builder.Arguments, builder.InputStreams, cancellationToken);
    }

    /// <summary>
    /// Returns the metadata of the given file using ffmpeg.
    /// </summary>
    /// <param name="arguments">The ffmpeg arguments.</param>
    /// <param name="inputStreams">The list of input streams.</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    public async Task<InputMetadata[]> GetMetadataAsync(string arguments, IList<InputStream>? inputStreams = null,
        CancellationToken cancellationToken = default)
    {
        var startInfo = new StartInfo()
        {
            Arguments = arguments,
            InputStreams = inputStreams,
        };
        var result = await ExecuteAsync(startInfo, cancellationToken);
        return result.Inputs;
    }

    /// <summary>
    /// Reads the FFmpeg output starting by an "Input#" line and returns the input information.
    /// </summary>
    /// <param name="intentReader">The current reader.</param>
    /// <returns>Returns the input metadata.</returns>
    private static async Task<InputMetadata> ReadInputMedaDataAsync(IntentTextReader intentReader)
    {
        var input = new InputMetadata();
        var streams = new List<StreamMetadata>();
        await foreach (var lineInput in intentReader.BeginAndReadBlockAsync())
        {
            if (lineInput.StartsWith("Metadata:"))
            {
                await foreach (var (name, value) in intentReader.BeginAndReadBlockPropertiesAsync())
                {
                    switch (name)
                    {
                        case "title":
                            input.Title = value;
                            break;
                        case "encoder":
                            input.Encoder = value;
                            break;
                    }
                }
            }
            else if (lineInput.StartsWith("Duration:"))
            {
                const int timeStart = 9;
                var timeEnd = lineInput.IndexOf(',', StringComparison.Ordinal);
                if (timeEnd < 0) continue;
                var timeText = lineInput.Substring(timeStart, timeEnd - timeStart).Trim();
                if (!TimeSpan.TryParse(timeText, out var time)) continue;
                input.Duration = time;
            }
            else if (lineInput.StartsWith("Chapters:"))
            {
                var chapters = new List<ChapterMetadata>();
                await foreach (var lineChapters in intentReader.BeginAndReadBlockAsync())
                {
                    var chapter = ReadChapterMetadataByLine(lineChapters);
                    await foreach (var lineChapter in intentReader.BeginAndReadBlockAsync())
                    {
                        if (lineChapter.StartsWith("Metadata:"))
                        {
                            await foreach (var (name, value) in intentReader.BeginAndReadBlockPropertiesAsync())
                            {
                                switch (name)
                                {
                                    case "title":
                                        chapter.Title = value;
                                        break;
                                }
                            }
                        }
                    }
                    chapters.Add(chapter);
                }

                input.Chapters = chapters.ToArray();
            }
            else if (lineInput.StartsWith("Stream #"))
            {
                var stream = CreateStreamMetadataByLine(lineInput);
                await foreach (var lineStream in intentReader.BeginAndReadBlockAsync())
                {
                    if (lineStream.StartsWith("Metadata:"))
                    {
                        await foreach (var (name, value) in intentReader.BeginAndReadBlockPropertiesAsync())
                        {
                            switch (name)
                            {
                                case "title":
                                    stream.Title = value;
                                    break;
                            }
                        }
                    }
                }
                
                streams.Add(stream);
            }
        }

        input.Streams = streams.ToArray();
        return input;
    }

    /// <summary>
    /// Reads the chapter metadata from the given line.
    /// </summary>
    /// <param name="line">The line that start with 'Chapter #'</param>
    /// <returns>Returns the stream metadata.</returns>
    private static ChapterMetadata ReadChapterMetadataByLine(string line)
    {
        var chapter = new ChapterMetadata();

        var indexInputId = line.IndexOf('#');
        if (indexInputId < 0) return chapter;
        
        var indexStreamId = line.IndexOf(':', indexInputId + 1);
        if (indexStreamId < 0) return chapter;

        chapter.InputId = ulong.Parse(line.Substring(indexInputId + 1, indexStreamId - indexInputId - 1));

        var indexEnd = line.IndexOf(':', indexStreamId + 1);
        if (indexEnd < 0) return chapter;
        
        chapter.Id = ulong.Parse(line.Substring(indexStreamId + 1, indexEnd - indexStreamId - 1));
        
        var indexChapterStart = line.IndexOf("start", indexEnd + 1, StringComparison.Ordinal);
        if (indexChapterStart < 0) return chapter;
        
        var indexChapterStartEnd = line.IndexOf(',', indexChapterStart + 1);
        if (indexChapterStartEnd < 0) return chapter;
        
        var chapterStartText = line.Substring(indexChapterStart + 6, indexChapterStartEnd - indexChapterStart - 6).Trim();
        if (double.TryParse(chapterStartText, CultureInfo.InvariantCulture, out var chapterStart))
            chapter.Start = TimeSpan.FromSeconds(chapterStart);
        
        var indexChapterEnd = line.IndexOf("end", indexChapterStartEnd + 1, StringComparison.Ordinal);
        if (indexChapterEnd < 0) return chapter;

        var chapterEndText = line.Substring(indexChapterEnd + 4).Trim();
        if (double.TryParse(chapterEndText, CultureInfo.InvariantCulture, out var chapterEnd))
            chapter.End = TimeSpan.FromSeconds(chapterEnd);
        
        return chapter;
    }
    
    /// <summary>
    /// Reads the stream metadata from the given line.
    /// </summary>
    /// <param name="line">The line that start with 'Stream #'</param>
    /// <returns>Returns the stream metadata.</returns>
    private static StreamMetadata CreateStreamMetadataByLine(string line)
    {
        var stream = new StreamMetadata();

        var indexInputId = line.IndexOf('#');
        if (indexInputId < 0) return stream;
        
        var indexStreamId = line.IndexOf(':', indexInputId + 1);
        if (indexStreamId < 0) return stream;
        
        stream.InputId = ulong.Parse(line.Substring(indexInputId + 1, indexStreamId - indexInputId - 1));
        
        var indexType = line.IndexOf(':', indexStreamId + 1);
        if (indexType < 0) return stream;

        var indexStreamIdEnd = indexType;
        
        // The transport stream pid: [0x0000]
        var indexPidStart = line.IndexOf('[', indexStreamId + 1);
        if (indexPidStart >= 0 && indexPidStart < indexType)
        {
            indexStreamIdEnd = indexPidStart;
            
            var indexPidEnd = line.IndexOf(']', indexPidStart + 1);
            if (indexPidEnd < 0) return stream;
            
            var pidStr = line.Substring(indexPidStart + 3, indexPidEnd - indexPidStart - 3);
            stream.Pid = (ushort)Convert.ToInt32(pidStr, 16);
        }
        
        // Language is not always available
        var indexLanguageStart = line.IndexOf('(', indexStreamId + 1);
        if (indexLanguageStart >= 0 && indexLanguageStart < indexType)
        {
            indexStreamIdEnd = indexLanguageStart;
            
            var indexLanguageEnd = line.IndexOf(')', indexLanguageStart + 1);
            if (indexLanguageEnd < 0) return stream;

            stream.Language = line.Substring(indexLanguageStart + 1, indexLanguageEnd - indexLanguageStart - 1);
        }
        
        var streamIdStr = line.Substring(indexStreamId + 1, indexStreamIdEnd - indexStreamId - 1);
        if (ulong.TryParse(streamIdStr, out var streamId))
            stream.Id = streamId;

        var indexFormat = line.IndexOf(':', indexType + 1);
        if (indexFormat < 0) return stream;

        var type = line.Substring(indexType + 1, indexFormat - indexType - 1).Trim();
        stream.Type = type switch
        {
            "Video" => StreamType.Video,
            "Audio" => StreamType.Audio,
            "Subtitle" => StreamType.Subtitle,
            "Data" => StreamType.Data,
            "Attachment" => StreamType.Attachment,
            _ => throw new ArgumentException($"Unknown stream type: '{type}'")
        };
        
        var indexEnd = line.IndexOf(',', indexFormat + 1);
        if (indexEnd < 0) indexEnd = line.Length;
        
        stream.Format = line.Substring(indexFormat + 1, indexEnd - indexFormat - 1).Trim();
        
        return stream;
    }
    
    #endregion Prope
}