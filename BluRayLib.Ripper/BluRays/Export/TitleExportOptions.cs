namespace BluRayLib.Ripper.BluRays.Export;

public class TitleExportOptions
{
    /// <summary>
    /// Gets the title to export.
    /// </summary>
    public TitleData Title { get; }

    private TitleExportOptions(TitleData title, string basename)
    {
        Title = title;
        Basename = basename;
    }
    
    #region Output
    
    /// <summary>
    /// Gets and sets the basename.
    /// </summary>
    public string Basename { get; set; }
    
    /// <summary>
    /// Gets and sets the video extension.
    /// </summary>
    public string Extension { get; set; } = ".mkv";

    /// <summary>
    /// Gets and sets the FFmpeg video format.
    /// </summary>
    public string? VideoFormat { get; set; } = "matroska";
    
    /// <summary>
    /// Gets and sets the codec options.
    /// </summary>
    public CodecOptions Codec { get; set; } = new();
    
    #endregion Output
    
    #region Streams
    
    /// <summary>
    /// Gets and sets the default audio track.
    /// </summary>
    public ushort DefaultAudioStreamId { get; set; }
    
    /// <summary>
    /// Gets and sets the default subtitle track.
    /// </summary>
    public ushort DefaultSubtitleStreamId { get; set; }
    
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
    /// Gets and sets the external stream filenames. If set and <see cref="ExportSubtitlesAsSeparateFiles"/> is enabled,
    /// this can overwrite the default subtitle filenames. Set ID 0 to overwrite the main video file.
    /// </summary>
    public TitleNameMap? NameMap { get; set; }

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
    
    #endregion Streams

    public static TitleExportOptions From(TitleData title, string basename)
    {
        var exporter = new TitleExportOptions(title, basename);

        // Find default streams
        if (title.Segments.Length > 0)
        {
            var firstSegment = title.Segments.First();
            foreach (var stream in firstSegment.AudioStreams)
            {
                exporter.DefaultAudioStreamId = stream.Id;
                break;
            }

            foreach (var stream in firstSegment.SubtitleStreams)
            {
                exporter.DefaultSubtitleStreamId = stream.Id;
                break;
            }
        }

        return exporter;
    }
}