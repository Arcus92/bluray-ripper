namespace BluRayLib.Ripper.BluRays.Export;

public class TitleExportOptions
{
    /// <summary>
    /// Gets the title to export.
    /// </summary>
    public TitleData Title { get; }

    private TitleExportOptions(TitleData title)
    {
        Title = title;
    }

    #region Codecs
    
    /// <summary>
    /// Gets and sets the FFmpeg video codec.
    /// </summary>
    public string VideoCodec { get; set; } = "copy";
    
    /// <summary>
    /// Gets and sets the FFmpeg audio codec.
    /// </summary>
    public string AudioCodec { get; set; } = "copy";
    
    /// <summary>
    /// Gets and sets the FFmpeg subtitle codec.
    /// </summary>
    public string SubtitleCodec { get; set; } = "copy";
    
    /// <summary>
    /// Gets and sets the FFmpeg constant rate factor.
    /// </summary>
    public int? ConstantRateFactor  { get; set; }
    
    /// <summary>
    /// Gets and sets the FFmpeg max bitrate.
    /// </summary>
    public int? MaxRate  { get; set; }
    
    /// <summary>
    /// Gets and sets the FFmpeg buffer size.
    /// </summary>
    public int? BufferSize  { get; set; }
    
    #endregion Codecs
    
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

    public static TitleExportOptions From(TitleData title)
    {
        var exporter = new TitleExportOptions(title);
        
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