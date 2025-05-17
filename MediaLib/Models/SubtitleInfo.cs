namespace MediaLib.Models;

public class SubtitleInfo : StreamInfo
{
    /// <summary>
    /// Gets the language code of this track.
    /// </summary>
    public string LanguageCode { get; set; } = "";
}