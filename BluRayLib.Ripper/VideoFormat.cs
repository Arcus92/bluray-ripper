namespace BluRayLib.Ripper;

public readonly struct VideoFormat
{
    public VideoFormat(string extension, string fFmpegFormat)
    {
        Extension = extension;
        FFmpegFormat = fFmpegFormat;
    }

    /// <summary>
    /// Gets the file extension, including the dot.
    /// </summary>
    public string Extension { get; }
    
    /// <summary>
    /// Gets the format name in FFmpeg.
    /// </summary>
    public string FFmpegFormat { get; }
    
    /// <summary>
    /// Gets if this format supports PGS subtitles.
    /// </summary>
    public bool SupportPgs { get; init; }

    #region Static
    
    /// <summary>
    /// The MP4 format.
    /// </summary>
    public static readonly VideoFormat Mp4 = new(".mp4", "mp4")
    {
        SupportPgs = false,
    };

    /// <summary>
    /// The MKV (Matroska) format.
    /// </summary>
    public static readonly VideoFormat Mkv = new(".mkv", "matroska")
    {
        SupportPgs = true,
    };
    
    /// <summary>
    /// The list of all formats.
    /// </summary>
    public static readonly VideoFormat[] All = [ Mp4, Mkv ];

    /// <summary>
    /// Returns the video format from the file extension.
    /// </summary>
    /// <param name="extension">The file extensions.</param>
    /// <returns></returns>
    public static VideoFormat? FromExtension(string extension)
    {
        foreach (var format in All)
        {
            if (string.Equals(format.Extension, extension, StringComparison.InvariantCultureIgnoreCase))
                return format;
        }
        return null;
    }
    
    #endregion Static
}