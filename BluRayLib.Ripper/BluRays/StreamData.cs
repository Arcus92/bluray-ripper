namespace BluRayLib.Ripper.BluRays;

public abstract class StreamData
{
    public StreamData(ushort id, string description)
    {
        Id = id;
        Description = description;
    }

    /// <summary>
    /// Gets the stream id.
    /// </summary>
    public ushort Id { get; }

    /// <summary>
    /// Gets the stream's description, like format type.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Gets if this track is a secondary track.
    /// </summary>
    public bool IsSecondary { get; init; }
    
    /// <summary>
    /// Gets if this track is the default track.
    /// </summary>
    public bool IsDefault { get; init; }
}