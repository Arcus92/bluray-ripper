using BluRayLib.PresentationGraphicStream.Segments;
using MediaLib.Utils.IO;

namespace BluRayLib.PresentationGraphicStream;

/// <summary>
/// The display set contains all information to display a graphic subtitle from a PGS (Presentation Graphic Stream).
/// </summary>
public class DisplaySet
{
    /// <summary>
    /// The file magic number.
    /// </summary>
    private const ushort MagicNumber = 0x5047;
    
    /// <summary>
    /// Gets the presentation timestamp in 90 kHz.
    /// </summary>
    public uint PresentationTimestamp { get; set; }
    
    /// <summary>
    /// Gets the decoding timestamp in 90 kHz.
    /// </summary>
    public uint DecodingTimestamp { get; set; }
    
    /// <summary>
    /// Gets the presentation composition.
    /// </summary>
    public PresentationCompositionSegment PresentationComposition { get; set; } = new();
    
    /// <summary>
    /// Gets the palette definitions.
    /// </summary>
    public List<PaletteDefinitionSegment> PaletteDefinitions { get; } = new();
    
    /// <summary>
    /// Gets the object definitions.
    /// </summary>
    public List<ObjectDefinitionSegment> ObjectDefinitions { get; } = new();
    
    /// <summary>
    /// Gets the window definitions.
    /// </summary>
    public List<WindowDefinitionSegment> WindowDefinitions { get; } = new();

    /// <summary>
    /// Clears and resets the current display set.
    /// </summary>
    public void Clear()
    {
        PresentationComposition = new PresentationCompositionSegment();
        PaletteDefinitions.Clear();
        ObjectDefinitions.Clear();
        WindowDefinitions.Clear();
    }
    
    /// <summary>
    /// Reads the PGS format.
    /// </summary>
    /// <param name="reader">The binary reader.</param>
    /// <param name="includeHeader">Should the header be included? The PG header is written before every segment.</param>
    public void Read(BigEndianBinaryReader reader, bool includeHeader = true)
    {
        PresentationComposition = new PresentationCompositionSegment();
        PaletteDefinitions.Clear();
        ObjectDefinitions.Clear();
        WindowDefinitions.Clear();
        
        while (true)
        {
            // The header is included before every segment. Even for the end segment.
            if (includeHeader)
            {
                var magicNumber = reader.ReadUInt16();
                if (magicNumber != MagicNumber) 
                    throw new InvalidDataException("Magic number mismatch. Expected: 0x5047.");
                PresentationTimestamp = reader.ReadUInt32();
                DecodingTimestamp = reader.ReadUInt32();
            }

            if (ReadSegment(reader) == EndSegment.Type)
                return;
        }
    }

    /// <summary>
    /// Reads the next segment of this display set.
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException"></exception>
    public byte ReadSegment(BigEndianBinaryReader reader)
    {
        var type = reader.ReadByte();
        var size = reader.ReadUInt16();
        var start = reader.Position;
        switch (type)
        {
            case PaletteDefinitionSegment.Type:
                var pds = new PaletteDefinitionSegment();
                pds.Read(reader, size);
                PaletteDefinitions.Add(pds);
                break;
            case ObjectDefinitionSegment.Type:
                var ods = new ObjectDefinitionSegment();
                ods.Read(reader, size);
                ObjectDefinitions.Add(ods);
                break;
            case PresentationCompositionSegment.Type:
                var pcs = new PresentationCompositionSegment();
                pcs.Read(reader, size);
                PresentationComposition = pcs;
                break;
            case WindowDefinitionSegment.Type:
                var wds = new WindowDefinitionSegment();
                wds.Read(reader, size);
                WindowDefinitions.Add(wds);
                break;
            default:
                throw new InvalidDataException($"Unknown segment type: {type}.");
        }

        // Check
        var realSize = reader.Position - start;
        if (size != realSize)
        {
            throw new InvalidDataException($"Segment size doesn't match: {type} - read: {realSize} - expected: {size}.");
        }
        
        return type;
    }

    /// <summary>
    /// Writes the PGS format.
    /// </summary>
    /// <param name="writer">The binary writer.</param>
    /// <param name="includeHeader">Should the header be included? The PG header is written before every segment.</param>
    public void Write(BigEndianBinaryWriter writer, bool includeHeader = true)
    {
        WriteSegment(PresentationComposition, writer, includeHeader);
        WriteSegments(WindowDefinitions, writer, includeHeader);
        WriteSegments(PaletteDefinitions, writer, includeHeader);
        WriteSegments(ObjectDefinitions, writer, includeHeader);
        WriteSegment(EndSegment.Shared, writer, includeHeader);
    }

    /// <summary>
    /// Writes all segments to the stream.
    /// </summary>
    /// <param name="segments">The segments to write.</param>
    /// <param name="writer">The writer.</param>
    /// <param name="includeHeader">Should the header be included?</param>
    /// <typeparam name="T">The segment type.</typeparam>
    private void WriteSegments<T>(IEnumerable<T> segments, BigEndianBinaryWriter writer, bool includeHeader = true)
        where T : IPresentationGraphicSegment
    {
        foreach (var segment in segments)
        {
            WriteSegment(segment, writer, includeHeader);
        }
    }

    /// <summary>
    /// Writes a segment to the stream.
    /// </summary>
    /// <param name="segment">The segment to write.</param>
    /// <param name="writer">The writer.</param>
    /// <param name="includeHeader">Should the header be included?</param>
    /// <typeparam name="T">The segment type.</typeparam>
    private void WriteSegment<T>(T segment, BigEndianBinaryWriter writer, bool includeHeader = true) where T : IPresentationGraphicSegment
    {
        if (includeHeader)
        {
            writer.Write(MagicNumber);
            writer.Write(PresentationTimestamp);
            writer.Write(DecodingTimestamp);
        }
            
        var length = segment.GetSegmentLength();
        writer.Write(T.Type);
        writer.Write(length);
        segment.Write(writer);
    }
}