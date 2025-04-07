using BluRayLib.Utils.IO;

namespace BluRayLib.PresentationGraphicStream.Segments;

public struct WindowDefinitionSegment : IPresentationGraphicSegment
{
    /// <summary>
    /// The type byte in the PGS.
    /// </summary>
    public const byte Type = 0x17;
    
    /// <inheritdoc />
    static byte IPresentationGraphicSegment.Type => Type;
    
    public WindowDefinitionSegment()
    {
        Windows = Array.Empty<WindowDefinition>();
    }

    public WindowDefinition[] Windows { get; set; }

    public struct WindowDefinition
    {
        public byte Id { get; set; }
        public ushort HorizontalPosition { get; set; }
        public ushort VerticalPosition { get; set; }
        public ushort Width { get; set; }
        public ushort Height { get; set; }
    }
    
    /// <inheritdoc />
    public void Read(BigEndianBinaryReader reader, ushort segmentLength)
    {
        var count = reader.ReadByte();
        Windows = new WindowDefinition[count];
        for (var i = 0; i < count; i++)
        {
            var window = new WindowDefinition();
            window.Id = reader.ReadByte();
            window.HorizontalPosition = reader.ReadUInt16();
            window.VerticalPosition = reader.ReadUInt16();
            window.Width = reader.ReadUInt16();
            window.Height = reader.ReadUInt16();

            Windows[i] = window;
        }
    }
    
    /// <inheritdoc />
    public void Write(BigEndianBinaryWriter writer)
    {
        writer.Write((byte)Windows.Length);
        foreach (var window in Windows)
        {
            writer.Write(window.Id);
            writer.Write(window.HorizontalPosition);
            writer.Write(window.VerticalPosition);
            writer.Write(window.Width);
            writer.Write(window.Height);
        }
    }
    
    /// <inheritdoc />
    public ushort GetSegmentLength()
    {
        return (ushort)(1 + Windows.Length * 9);
    }
}