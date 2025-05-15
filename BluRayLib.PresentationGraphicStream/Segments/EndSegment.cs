using MediaLib.Utils.IO;

namespace BluRayLib.PresentationGraphicStream.Segments;

public class EndSegment : IPresentationGraphicSegment
{
    /// <summary>
    /// The type byte in the PGS.
    /// </summary>
    public const byte Type = 0x80;
    
    /// <inheritdoc />
    static byte IPresentationGraphicSegment.Type => Type;

    /// <summary>
    /// Gets the shared instance for the end segment.
    /// </summary>
    public static EndSegment Shared { get; } = new();

    /// <inheritdoc />
    public void Read(BigEndianBinaryReader reader, ushort segmentLength)
    {
    }
    
    /// <inheritdoc />
    public void Write(BigEndianBinaryWriter writer)
    {
    }
    
    /// <inheritdoc />
    public ushort GetSegmentLength()
    {
        return 0;
    }
}