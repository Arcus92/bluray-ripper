using BluRayLib.Utils.IO;

namespace BluRayLib.PresentationGraphicStream.Segments;

/// <summary>
/// A segment in a presentation graphic stream.
/// </summary>
public interface IPresentationGraphicSegment
{
    /// <summary>
    /// Reads this segment from the given stream.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="segmentLength">The length of the segment.</param>
    void Read(BigEndianBinaryReader reader, ushort segmentLength);

    /// <summary>
    /// Writes this segment to the given stream.
    /// </summary>
    /// <param name="writer">The writer.</param>
    void Write(BigEndianBinaryWriter writer);
    
    /// <summary>
    /// Returns the binary length of the segment when written to a file.
    /// </summary>
    /// <returns></returns>
    ushort GetSegmentLength();

    /// <summary>
    /// Gets the type byte for this segment.
    /// </summary>
    static abstract byte Type { get; }
}