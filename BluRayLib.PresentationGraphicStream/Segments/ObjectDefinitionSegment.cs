using BluRayLib.Utils.IO;

namespace BluRayLib.PresentationGraphicStream.Segments;

public struct ObjectDefinitionSegment : IPresentationGraphicSegment
{
    /// <summary>
    /// The type byte in the PGS.
    /// </summary>
    public const byte Type = 0x15;

    /// <inheritdoc />
    static byte IPresentationGraphicSegment.Type => Type;

    public ObjectDefinitionSegment()
    {
        Id = 0;
        VersionNumber = 0;
        LastInSequenceFlag = 0;
        Width = 0;
        Height = 0;
        Data = Array.Empty<byte>();
    }
    
    public ushort Id { get; set; }
    public byte VersionNumber { get; set; }
    public byte LastInSequenceFlag { get; set; }
    public ushort Width { get; set; }
    public ushort Height { get; set; }
    public int DataLength { get; set; }
    public byte[] Data { get; set; }

    private const byte LastInSequence = 0x40;
    private const byte FirstInSequence = 0x80;
    private const byte FirstAndLastInSequence = LastInSequence | FirstInSequence;

    /// <summary>
    /// Gets if this is the first element in the sequence.
    /// </summary>
    public bool IsFirstInSequence => (LastInSequenceFlag & FirstInSequence) != 0;
    
    /// <summary>
    /// Gets if this is the last element in the sequence.
    /// </summary>
    public bool IsLastInSequence => (LastInSequenceFlag & LastInSequence) != 0;

    /// <inheritdoc />
    public void Read(BigEndianBinaryReader reader, ushort segmentLength)
    {
        Id = reader.ReadUInt16();
        VersionNumber = reader.ReadByte();
        LastInSequenceFlag = reader.ReadByte();
        
        // This is the total data length of ALL segments.
        // We need to add all data segments to encode the image.
        if (IsFirstInSequence)
        {
            DataLength = reader.ReadUInt24();
            Width = reader.ReadUInt16();
            Height = reader.ReadUInt16();
            Data = reader.ReadBytes(segmentLength - 11);
        }
        else
        {
            Data = reader.ReadBytes(segmentLength - 4);
        }
    }

    /// <inheritdoc />
    public void Write(BigEndianBinaryWriter writer)
    {
        writer.Write(Id);
        writer.Write(VersionNumber);
        writer.Write(LastInSequenceFlag);
        if (IsFirstInSequence)
        {
            writer.WriteUInt24(DataLength);
            writer.Write(Width);
            writer.Write(Height);
            writer.Write(Data);
        }
        else
        {
            writer.Write(Data);
        }
    }

    /// <inheritdoc />
    public ushort GetSegmentLength()
    {
        if (IsFirstInSequence)
        {
            return (ushort)(11 + Data.Length);
        }
        else
        {
            return (ushort)(4 + Data.Length);
        }
    }
}