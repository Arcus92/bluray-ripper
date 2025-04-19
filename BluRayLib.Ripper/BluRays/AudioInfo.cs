namespace BluRayLib.Ripper.BluRays;

public class AudioInfo : StreamData
{
    public AudioInfo(ushort id) : base(id)
    {
    }
    
    /// <summary>
    /// Gets the language code of this track.
    /// </summary>
    public string LanguageCode { get; set; } = "";
    
    public override string ToString() => $"Audio [0x{Id:x4}] - [{LanguageCode}] {(IsSecondary ?" (secondary)" : "")}";
}