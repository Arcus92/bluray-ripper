using BluRayLib.Utils;
using BluRayLib.Utils.IO;

namespace BluRayLib.Mpls;

public class SubPlayItem
{
    /// <summary>
    /// Gets the item id.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Gets the file type.
    /// </summary>
    public string Type { get; set; } = "";
    
    public byte ConnectionCondition { get; set; }
    public bool IsMultiClipEntries { get; set; }
    
    public ushort StcId { get; set; }
    public uint InTime { get; set; }
    public uint OutTime { get; set; }
    
    public ushort SyncPlayItemId { get; set; }
    public uint SyncStartPts { get; set; }
    
    public uint Duration => OutTime - InTime;

    public MultiClipEntry[] MultiClipEntries { get; set; } = [];

    /// <summary>
    /// Gets the mask table.
    /// </summary>
    public MaskTable MaskTable { get; } = new();
    
    public void Read(BigEndianBinaryReader reader)
    {
        var length = reader.ReadUInt16();
        var start = reader.Position;
        
        Name = reader.ReadString(5);
        Type = reader.ReadString(4);
        reader.Skip(3);
        var value = reader.ReadByte();
        ConnectionCondition = BitUtils.GetBitsFromLeft(value, 3, 4);
        IsMultiClipEntries = BitUtils.GetBitFromLeft(value, 7);
        StcId = reader.ReadByte();
        InTime = reader.ReadUInt32();
        OutTime = reader.ReadUInt32();
        SyncPlayItemId = reader.ReadUInt16();
        SyncStartPts = reader.ReadUInt32();

        if (IsMultiClipEntries)
        {
            var count = reader.ReadByte();
            reader.Skip(1);
            MultiClipEntries = new MultiClipEntry[count];
            for (var i = 0; i < count; i++)
            {
                var entry = new MultiClipEntry();
                entry.Read(reader);
                MultiClipEntries[i] = entry;
            }
        }
        
        reader.SkipTo(start + length);
    }
}