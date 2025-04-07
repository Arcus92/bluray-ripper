using BluRayLib.Utils.IO;

namespace BluRayLib.PresentationGraphicStream.Segments;

public struct PaletteDefinitionSegment : IPresentationGraphicSegment
{
    /// <summary>
    /// The type byte in the PGS.
    /// </summary>
    public const byte Type = 0x14;
    
    /// <inheritdoc />
    static byte IPresentationGraphicSegment.Type => Type;
    
    public PaletteDefinitionSegment()
    {
        Id = 0;
        VersionNumber = 0;
        Entries = new Dictionary<byte, Entry>();
    }

    public byte Id { get; set; }
    public byte VersionNumber { get; set; }
    
    public Dictionary<byte, Entry> Entries { get; set; }

    public readonly struct Entry
    { 
        public byte Y { get; init; }
        public byte Cr { get; init; }
        public byte Cb { get; init; }
        public byte A { get; init; }
    }
    
    /// <inheritdoc />
    public void Read(BigEndianBinaryReader reader, ushort segmentLength)
    {
        Id = reader.ReadByte();
        VersionNumber = reader.ReadByte();
        
        var count = (segmentLength - 2) / 5;
        Entries = new Dictionary<byte, Entry>();
        for (var i = 0; i < count; i++)
        {
            var id = reader.ReadByte();
            var entry = new Entry()
            {
                Y = reader.ReadByte(),
                Cr = reader.ReadByte(),
                Cb = reader.ReadByte(),
                A = reader.ReadByte()
            };
            Entries.Add(id, entry);
        }
    }
    
    /// <inheritdoc />
    public void Write(BigEndianBinaryWriter writer)
    {
        writer.Write(Id);
        writer.Write(VersionNumber);
        foreach (var pair in Entries)
        {
            writer.Write(pair.Key);
            writer.Write(pair.Value.Y);
            writer.Write(pair.Value.Cr);
            writer.Write(pair.Value.Cb);
            writer.Write(pair.Value.A);
        }
    }
    
    /// <inheritdoc />
    public ushort GetSegmentLength()
    {
        return (ushort)(2 + Entries.Count * 5);
    }
}