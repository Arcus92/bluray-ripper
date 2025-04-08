using BluRayLib.Utils;
using BluRayLib.Utils.IO;

namespace BluRayLib.Mpls;

public class PlaylistItem
{
    /// <summary>
    /// Gets the item id.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Gets the file type.
    /// </summary>
    public string Type { get; set; } = "";

    /// <summary>
    /// Gets and sets if this is a multi angle playlist.
    /// </summary>
    public bool IsMultiAngle { get; set; }

    public ushort StcId { get; set; }

    public uint InTime { get; set; }
    public uint OutTime { get; set; }
    
    public uint Duration => OutTime - InTime;

    /// <summary>
    /// Gets the mask table.
    /// </summary>
    public MaskTable MaskTable { get; } = new();
    
    public bool PlayItemRandomAccess { get; set; }
    public byte StillMode { get; set; }
    public ushort StillTime { get; set; }

    /// <summary>
    /// Gets the stream-number-table.
    /// </summary>
    public StreamNumberTable StreamNumberTable { get; } = new();
     
    public void Read(BigEndianBinaryReader reader)
    {
        var length = reader.ReadUInt16();
        var start = reader.Position;
        
        Name = reader.ReadString(5);
        Type = reader.ReadString(4);

        var miscFlags = reader.ReadBits16();
        miscFlags.Skip(12);
        IsMultiAngle = miscFlags.ReadBit();
        StcId = reader.ReadByte();
        InTime = reader.ReadUInt32();
        OutTime = reader.ReadUInt32();
        
        MaskTable.Read(reader);
        
        PlayItemRandomAccess = reader.ReadByte() == 1;
        StillMode = reader.ReadByte();
        StillTime = reader.ReadUInt16();

        if (IsMultiAngle)
        {
            throw new NotImplementedException();
        }
        
        StreamNumberTable.Read(reader);
        
        reader.SkipTo(start + length);
    }
}