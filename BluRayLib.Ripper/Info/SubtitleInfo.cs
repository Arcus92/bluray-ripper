namespace BluRayLib.Ripper.Info;

public class SubtitleInfo
{
    public SubtitleInfo(ushort id)
    {
        Id = id;
    }

    /// <summary>
    /// Gets the track id.
    /// </summary>
    public ushort Id { get; }
    
    /// <summary>
    /// Gets the track index.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// Gets the language code of this track.
    /// </summary>
    public string LanguageCode { get; set; } = "";

    /// <summary>
    /// Gets if this track is a secondary track.
    /// </summary>
    public bool IsSecondary { get; set; }

    public override string ToString() => $"Subtitle #{Index} [0x{Id:x4}] - [{LanguageCode}] {(IsSecondary ?" (secondary)" : "")}";
}