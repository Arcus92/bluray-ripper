using MediaLib.Utils.IO;

namespace BluRayLib.Mpls;

/// <summary>
/// A playlist stream.
/// </summary>
public class PlaylistStream
{
    /// <summary>
    /// Gets the stream entry.
    /// </summary>
    public StreamEntry Entry { get; } = new();
    
    /// <summary>
    /// Gets the stream attributes.
    /// </summary>
    public StreamAttributes Attributes { get; } = new();
    
    public void Read(BigEndianBinaryReader reader)
    {
        Entry.Read(reader);
        Attributes.Read(reader);
    }
}