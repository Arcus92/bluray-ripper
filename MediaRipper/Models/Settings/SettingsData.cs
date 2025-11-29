using System;

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
    /// Gets and sets the custom FFmpeg path
    /// </summary>
    public string? FFmpegPath { get; set; } 

    /// <summary>
    /// Gets TheMovieDatabase settings.
    /// </summary>
    public TheMovieDatabaseSettings TheMovieDatabase { get; set; } = new();
}