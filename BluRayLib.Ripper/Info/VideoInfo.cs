namespace BluRayLib.Ripper.Info;

public class VideoInfo
{
    public VideoInfo(ushort id)
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
    /// Gets if this track is a secondary track.
    /// </summary>
    public bool IsSecondary { get; set; }
    
    public override string ToString() => $"Video #{Index} [0x{Id:x4}] {(IsSecondary ?" (secondary)" : "")}";
}