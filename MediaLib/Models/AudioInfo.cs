namespace MediaLib.Models;

public class AudioInfo : StreamInfo
{
    public AudioInfo(ushort id, string description) : base(id, description)
    {
    }
    
    /// <summary>
    /// Gets the language code of this track.
    /// </summary>
    public string LanguageCode { get; set; } = "";
    
    public override string ToString() => $"{Description} [0x{Id:x4}] - [{LanguageCode}]{(IsSecondary ?" (secondary)" : "")}{(IsDefault ?" (default)" : "")}";
}