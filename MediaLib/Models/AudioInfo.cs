namespace MediaLib.Models;

public class AudioInfo : StreamInfo
{
    /// <summary>
    /// Gets the language code of this track.
    /// </summary>
    public string LanguageCode { get; init; } = "";
}