namespace MediaLib.Formats;

/// <summary>
/// Provides a list of subtitle formats.
/// </summary>
public static class SubtitleFormats
{
    /// <summary>
    /// The PGS (presentation graphic stream) format.
    /// </summary>
    public static readonly MediaFormat Pgs = new()
    {
        Extension = ".sup",
        FFmpegFormat = "sup"
    };
    
    /// <summary>
    /// The Subrip format.
    /// </summary>
    public static readonly MediaFormat Subrip = new()
    {
        Extension = ".srt",
        FFmpegFormat = "srt"
    };
    
    /// <summary>
    /// The list of all formats.
    /// </summary>
    public static readonly MediaFormat[] All = [ Pgs, Subrip ];
}