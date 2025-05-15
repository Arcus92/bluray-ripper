using MediaLib.Utils.IO;

namespace BluRayLib.PresentationGraphicStream.Segments;

public struct PresentationCompositionSegment : IPresentationGraphicSegment
{
    /// <summary>
    /// The type byte in the PGS.
    /// </summary>
    public const byte Type = 0x16;
    
    /// <inheritdoc />
    static byte IPresentationGraphicSegment.Type => Type;
    public PresentationCompositionSegment()
    {
        Width = 0;
        Height = 0;
        FrameRate = 0;
        CompositionNumber = 0;
        CompositionState = 0;
        PaletteUpdateFlag = 0;
        PaletteId = 0;
        CompositionObjects = Array.Empty<CompositionObject>();
    }
    
    

    public ushort Width { get; set; }
    public ushort Height { get; set; }
    public byte FrameRate { get; set; }
    public ushort CompositionNumber { get; set; }
    public byte CompositionState { get; set; }
    public byte PaletteUpdateFlag { get; set; }
    public byte PaletteId { get; set; }
    
    public CompositionObject[] CompositionObjects { get; set; }
    public struct CompositionObject
    {
        public ushort Id { get; set; }
        public byte WindowId { get; set; }
        public byte CroppedFlag { get; set; }
        public ushort HorizontalPosition { get; set; }
        public ushort VerticalPosition { get; set; }
        public ushort CroppingHorizontalPosition { get; set; }
        public ushort CroppingVerticalPosition { get; set; }
        public ushort CroppingWidth { get; set; }
        public ushort CroppingHeightPosition { get; set; }
        public bool HasCropping => (CroppedFlag & 0x80) != 0;
    }
    
    /// <inheritdoc />
    public void Read(BigEndianBinaryReader reader, ushort segmentLength)
    {
        Width = reader.ReadUInt16();
        Height = reader.ReadUInt16();
        FrameRate = reader.ReadByte();
        CompositionNumber = reader.ReadUInt16();
        CompositionState = reader.ReadByte();
        PaletteUpdateFlag = reader.ReadByte();
        PaletteId = reader.ReadByte();
        
        var count = reader.ReadByte();
        CompositionObjects = new CompositionObject[count];
        for (var i = 0; i < count; i++)
        {
            var compositionObject = new CompositionObject();
            compositionObject.Id = reader.ReadUInt16();
            compositionObject.WindowId = reader.ReadByte();
            compositionObject.CroppedFlag = reader.ReadByte();
            compositionObject.HorizontalPosition = reader.ReadUInt16();
            compositionObject.VerticalPosition = reader.ReadUInt16();
            if (compositionObject.HasCropping)
            {
                compositionObject.CroppingHorizontalPosition = reader.ReadUInt16();
                compositionObject.CroppingVerticalPosition = reader.ReadUInt16();
                compositionObject.CroppingWidth = reader.ReadUInt16();
                compositionObject.CroppingHeightPosition = reader.ReadUInt16();
            }

            CompositionObjects[i] = compositionObject;
        }
    }
    
    /// <inheritdoc />
    public void Write(BigEndianBinaryWriter writer)
    {
        writer.Write(Width);
        writer.Write(Height);
        writer.Write(FrameRate);
        writer.Write(CompositionNumber);
        writer.Write(CompositionState);
        writer.Write(PaletteUpdateFlag);
        writer.Write(PaletteId);
        writer.Write((byte)CompositionObjects.Length);

        foreach (var compositionObject in CompositionObjects)
        {
            writer.Write(compositionObject.Id);
            writer.Write(compositionObject.WindowId);
            writer.Write(compositionObject.CroppedFlag);
            writer.Write(compositionObject.HorizontalPosition);
            writer.Write(compositionObject.VerticalPosition);
            if (compositionObject.HasCropping)
            {
                writer.Write(compositionObject.CroppingHorizontalPosition);
                writer.Write(compositionObject.CroppingVerticalPosition);
                writer.Write(compositionObject.CroppingWidth);
                writer.Write(compositionObject.CroppingHeightPosition);
            }
        }
    }
    
    /// <inheritdoc />
    public ushort GetSegmentLength()
    {
        var length = 11;
        foreach (var compositionObject in CompositionObjects)
        {
            length += compositionObject.HasCropping ? 16 : 8;
        }

        return (ushort)length;
    }
}