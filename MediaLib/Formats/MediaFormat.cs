namespace MediaLib.Formats;

/// <summary>
/// Describes a format type.
/// </summary>
public readonly struct MediaFormat
{
    /// <summary>
    /// Gets the file extension, including the dot.
    /// </summary>
    public required string Extension { get; init; }
    
    /// <summary>
    /// Gets the format name in FFmpeg.
    /// </summary>
    public required string FFmpegFormat { get; init; }
}