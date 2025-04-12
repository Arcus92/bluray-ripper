using BluRayLib.FFmpeg;
using BluRayLib.Ripper.Info;

namespace BluRayLib.Ripper.Export;

public class PlaylistExporter
{
    /// <summary>
    /// The BluRay instance.
    /// </summary>
    private readonly BluRay _bluRay;

    /// <summary>
    /// The playlist id.
    /// </summary>
    public ushort PlaylistId { get; }

    /// <summary>
    /// Gets the playlist info object.
    /// </summary>
    public PlaylistInfo Playlist { get; }

    public PlaylistExporter(BluRay bluRay, ushort playlistId)
    {
        _bluRay = bluRay;
        PlaylistId = playlistId;
        Playlist = _bluRay.GetPlaylistInfo(PlaylistId);
        InitDefaultStreams();
    }

    #region Stream options
    
    /// <summary>
    /// Gets and sets the default audio track.
    /// </summary>
    public ushort DefaultAudioStreamId { get; set; }
    
    /// <summary>
    /// Gets and sets the default subtitle track.
    /// </summary>
    public ushort DefaultSubtitleStreamId { get; set; }
    
    private void InitDefaultStreams()
    {
        if (Playlist.Segments.Length == 0) return;
        var firstSegment = Playlist.Segments.First();
        foreach (var stream in firstSegment.AudioStreams)
        {
            DefaultAudioStreamId = stream.Id;
            break;
        }
        foreach (var stream in firstSegment.SubtitleStreams)
        {
            DefaultSubtitleStreamId = stream.Id;
            break;
        }
    }

    /// <summary>
    /// Gets and sets if the subtitles should be exported as separate files.
    /// This must be set if exported as MP4 because MP4 doesn't support PGS subtitles.
    /// </summary>
    public bool ExportSubtitlesAsSeparateFiles { get; set; }

    /// <summary>
    /// Gets and sets if the chapters should be exported.
    /// </summary>
    public bool ExportChapters { get; set; } = true;

    /// <summary>
    /// Gets and sets the list of the streams to ignore.
    /// </summary>
    public List<ushort>? IgnoredStreamIds { get; set; }

    /// <summary>
    /// Adds the given stream id to <see cref="IgnoredStreamIds"/>.
    /// This stream will be ignored when the playlist is exported.
    /// </summary>
    /// <param name="streamId">The stream id.</param>
    public void IgnoreStreamId(ushort streamId)
    {
        IgnoredStreamIds ??= [];
        IgnoredStreamIds.Add(streamId);
    }

    #endregion Stream options
    
    #region Command builder
    
    /// <summary>
    /// Gets and sets the FFmpeg command builder function for the video output.
    /// Use this to define your output codec.
    /// By default, <see cref="DefaultVideoCommandBuilder"/> is used.
    /// </summary>
    public Action<CommandBuilder> VideoCommandBuilder { get; set; } = DefaultVideoCommandBuilder;

    /// <summary>
    /// Gets and sets the FFmpeg command builder function for the subtitle outputs.
    /// This is only executed when <see cref="ExportSubtitlesAsSeparateFiles"/> is set.
    /// By default, <see cref="DefaultSubtitleCommandBuilder"/> is used.
    /// </summary>
    public Action<CommandBuilder> SubtitleCommandBuilder { get; set; } = DefaultSubtitleCommandBuilder;

    /// <summary>
    /// The default video command builder used by <see cref="VideoCommandBuilder"/>.
    /// </summary>
    /// <param name="builder"></param>
    public static void DefaultVideoCommandBuilder(CommandBuilder builder)
    {
        builder.Codec(StreamType.Video, "copy");
        builder.Codec(StreamType.Audio, "copy");
        builder.Codec(StreamType.Subtitle, "copy");
    }
    
    /// <summary>
    /// The default video command builder used by <see cref="SubtitleCommandBuilder"/>.
    /// </summary>
    /// <param name="builder"></param>
    public static void DefaultSubtitleCommandBuilder(CommandBuilder builder)
    {
        builder.Codec(StreamType.Subtitle, "copy");
    }
    
    #endregion Command builder

    #region Export
    
    /// <summary>
    /// Exports the playlist as a video file using FFmpeg.
    /// </summary>
    /// <param name="output">The output file.</param>
    /// <param name="outputFormat">The FFmpeg output format.</param>
    /// <param name="onUpdate">The status update event.</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task ExportAsync(string output, string? outputFormat = null, Action<ConverterUpdate>? onUpdate = null,
        CancellationToken cancellationToken = default)
    {
        var directory = Path.GetDirectoryName(output)!;
        var fileName = Path.GetFileNameWithoutExtension(output);
        
        var ffmpeg = new Engine();
        await ffmpeg.ConvertAsync(builder =>
        {
            // Builds the concat text file in memory
            var concatStream = new MemoryStream();
            var concatWriter = new StreamWriter(concatStream);
            foreach (var segment in Playlist.Segments)
            {
                var inputStream = builder.CreateInputStream(() => _bluRay.GetM2TsStream(segment.Id));
                concatWriter.WriteLine($"file '{inputStream.GetPath()}'");
            }
            concatWriter.Flush();
            concatStream.Position = 0;

            builder.Format("concat");
            builder.Safe(0);
            var input = builder.Input(concatStream);
            
            if (ExportChapters)
            {
                // Builds the chapter file in memory
                var chapterStream = new MemoryStream();
                var chapterWriter = new StreamWriter(chapterStream);
                foreach (var chapter in Playlist.Chapters)
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
            VideoCommandBuilder(builder);
            
            // Map the output streams
            var outputStreamCount = 0;
            var firstSegment = Playlist.Segments.First();
            foreach (var stream in firstSegment.VideoStreams)
            {
                if (IgnoredStreamIds?.Contains(stream.Id) ?? false) continue;
                builder.Map(input, stream.Index);
                outputStreamCount++;
            }
            foreach (var stream in firstSegment.AudioStreams)
            {
                if (IgnoredStreamIds?.Contains(stream.Id) ?? false) continue;
                builder.Map(input, stream.Index);
                if (!string.IsNullOrEmpty(stream.LanguageCode))
                    builder.Metadata(outputStreamCount, "language", stream.LanguageCode);
                if (DefaultAudioStreamId == stream.Id)
                    builder.Disposition(outputStreamCount, "default");

                outputStreamCount++;
            }
            if (!ExportSubtitlesAsSeparateFiles) // Only included if not exported as separate files...
            {
                foreach (var stream in firstSegment.SubtitleStreams)
                {
                    if (IgnoredStreamIds?.Contains(stream.Id) ?? false) continue;
                    builder.Map(input, stream.Index);
                    if (!string.IsNullOrEmpty(stream.LanguageCode))
                        builder.Metadata(outputStreamCount, "language", stream.LanguageCode);
                    if (DefaultSubtitleStreamId == stream.Id)
                        builder.Disposition(outputStreamCount, "default");
                    
                    outputStreamCount++;
                }
            }
            
            // Video output
            builder.OverwriteOutput();
            if (outputFormat is not null)
                builder.Format(outputFormat);
            builder.Output(output);
            
            
            // FFmpeg supports multiple outputs. We can export the subtitle files in a single run as well.
            // We just need to create a new mapping and then define a new output.
            if (ExportSubtitlesAsSeparateFiles)
            {
                // Export subtitle files
                foreach (var stream in firstSegment.SubtitleStreams)
                {
                    if (IgnoredStreamIds?.Contains(stream.Id) ?? false) continue;
                    
                    builder.Map(input, stream.Index);
                    builder.Metadata(0, "language", stream.LanguageCode);
                    // Single exports don't need a default flag.
                    
                    SubtitleCommandBuilder(builder);

                    // Subtitle output
                    builder.OverwriteOutput();
                    var path = Path.Combine(directory, $"{fileName}.{stream.LanguageCode}.{stream.Id}.sup");
                    builder.Output(path);
                }
            }
        }, onUpdate, cancellationToken);
    }
    
    #endregion Export
}