namespace BluRayLib.Ripper.BluRays;

public abstract class StreamData
{
    public StreamData(ushort id)
    {
        Id = id;
    }

    /// <summary>
    /// Gets the stream id.
    /// </summary>
    public ushort Id { get; }
    
    /// <summary>
    /// Gets if this track is a secondary track.
    /// </summary>
    public bool IsSecondary { get; set; }
}