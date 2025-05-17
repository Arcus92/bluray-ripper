namespace MediaLib.Models;

public abstract class StreamInfo : IMediaInfo
{
    /// <summary>
    /// Gets the stream id.
    /// </summary>
    public required ushort Id { get; init; }

    /// <summary>
    /// Gets the stream's description, like a format type.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets if this track is a secondary track.
    /// </summary>
    public bool IsSecondary { get; init; }
    
    /// <summary>
    /// Gets if this track is the default track.
    /// </summary>
    public bool IsDefault { get; init; }

    /// <inheritdoc />
    public override string ToString() => Name;
}