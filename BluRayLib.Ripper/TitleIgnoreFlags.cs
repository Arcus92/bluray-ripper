namespace BluRayLib.Ripper;

/// <summary>
/// Flags to indicate no-gos for tiles and flags them as to-be-ignored.
/// For example titles that are too short or doesn't contain audio are most likely used by menus.
/// </summary>
[Flags]
public enum TitleIgnoreFlags
{
    None = 0,
    
    /// <summary>
    /// The title is too short.
    /// </summary>
    TooShort = 1 << 0,
    
    /// <summary>
    /// The title is too long. Maybe a looping menu.
    /// </summary>
    TooLong = 1 << 1,
    
    /// <summary>
    /// The title is a menu.
    /// </summary>
    Menu = 1 << 2,
    
    /// <summary>
    /// The title doesn't have audio.
    /// </summary>
    NoAudio = 1 << 3,
    
    /// <summary>
    /// The title doesn't have subtitles.
    /// </summary>
    NoSubtitle = 1 << 4,
    
    /// <summary>
    /// The title repeats the same segment.
    /// </summary>
    RepeatingClips = 1 << 5,
    
    /// <summary>
    /// The title is a duplicate of an earlier title on disk.
    /// </summary>
    Duplicate = 1 << 6,
}