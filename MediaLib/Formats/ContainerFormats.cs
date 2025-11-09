namespace MediaLib.Formats;

/// <summary>
/// Provides a list of video container formats.
/// </summary>
public static class ContainerFormats
{
    /// <summary>
    /// The MP4 format.
    /// </summary>
    public static readonly MediaFormat Mp4 = new()
    {
        Extension = ".mp4",
        FFmpegFormat = "mp4"
    };

    /// <summary>
    /// The MKV (Matroska) format.
    /// </summary>
    public static readonly MediaFormat Mkv = new()
    {
        Extension = ".mkv",
        FFmpegFormat = "matroska"
    };
    
    /// <summary>
    /// The list of all formats.
    /// </summary>
    public static readonly MediaFormat[] All = [ Mp4, Mkv ];

    /// <summary>
    /// Returns if the given subtitle format is supported by the video container format.
    /// </summary>
    /// <param name="containerFormat">The video format to check.</param>
    /// <param name="subtitleFormat">The subtitle format to check.</param>
    /// <returns>Returns true, if the given subtitle is supported by the video container.</returns>
    public static bool SupportSubtitle(MediaFormat containerFormat, MediaFormat subtitleFormat)
    {
        return SupportSubtitle(containerFormat.FFmpegFormat, subtitleFormat.FFmpegFormat);
    }
    
    /// <summary>
    /// Returns if the given subtitle format is supported by the video container format.
    /// </summary>
    /// <param name="containerFormat">The video format to check.</param>
    /// <param name="subtitleFormat">The subtitle format to check.</param>
    /// <returns>Returns true, if the given subtitle is supported by the video container.</returns>
    public static bool SupportSubtitle(string containerFormat, string subtitleFormat)
    {
        return subtitleFormat switch
        {
            "sup" => containerFormat switch
            {
                "matroska" => true,
                _ => false
            },
            "subrip" => containerFormat switch
            {
                "matroska" => true,
                _ => false
            },
            _ => false
        };
    }
}