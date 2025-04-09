namespace BluRayRipper.Models;

public class OutputFormat
{
    public OutputFormat(string fileExtension, string fFmpegFormat)
    {
        FileExtension = fileExtension;
        FFmpegFormat = fFmpegFormat;
    }

    /// <summary>
    /// Gets the file extension, including the dot.
    /// </summary>
    public string FileExtension { get; }
    
    /// <summary>
    /// Gets the format name in FFmpeg.
    /// </summary>
    public string FFmpegFormat { get; }

    #region Static
    
    /// <summary>
    /// The MP4 format.
    /// </summary>
    public static readonly OutputFormat Mp4 = new(".mp4", "mp4");
    
    /// <summary>
    /// The MKV (Matroska) format.
    /// </summary>
    public static readonly OutputFormat Mkv = new(".mkv", "matroska");
    
    /// <summary>
    /// The list of all formats.
    /// </summary>
    public static readonly OutputFormat[] All = [ Mp4, Mkv ];
    
    #endregion Static
}