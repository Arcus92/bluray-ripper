using System;
using System.Runtime.InteropServices;

namespace MediaRipper.Models.Settings;

/// <summary>
/// The application settings data used by <see cref="Services.SettingService"/>.
/// </summary>
[Serializable]
public class SettingsData
{
    /// <summary>
    /// Gets and sets the last opened source path.
    /// </summary>
    public string? SourcePath { get; set; }

    /// <summary>
    /// Gets and sets the last opened output path.
    /// </summary>
    public string? OutputPath { get; set; }

    /// <summary>
    /// Gets and sets the custom FFmpeg path.
    /// </summary>
    public string? FFmpegPath { get; set; } = DefaultFFmpegPath();

    /// <summary>
    /// Gets and sets the video player path (FFplay or mpv).
    /// </summary>
    public string? FFplayPath { get; set; } = DefaultFFplayPath();
    
    /// <summary>
    /// Gets TheMovieDatabase settings.
    /// </summary>
    public TheMovieDatabaseSettings TheMovieDatabase { get; set; } = new();

    /// <summary>
    /// Gets the default FFmpeg path for the current platform.
    /// </summary>
    /// <returns></returns>
    private static string DefaultFFmpegPath()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "ffmpeg.exe" : "ffmpeg";
    }
    
    /// <summary>
    /// Gets the default FFplay path for the current platform.
    /// </summary>
    /// <returns></returns>
    private static string DefaultFFplayPath()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "ffplay.exe" : "ffplay";
    }
}