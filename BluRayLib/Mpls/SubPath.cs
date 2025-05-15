using BluRayLib.Enums;
using MediaLib.Utils.IO;

namespace BluRayLib.Mpls;

public class SubPath
{
    public SubPathType Type { get; set; }
    public bool IsRepeatSubPath { get; set; }

    public SubPlayItem[] Items { get; set; } = [];
    public void Read(BigEndianBinaryReader reader)
    {
        var length = reader.ReadUInt32();
        var start = reader.Position;
        
        reader.Skip(1); // Reserved
        Type = (SubPathType)reader.ReadByte();
        var miscFlags = reader.ReadUInt16();
        IsRepeatSubPath = (miscFlags & 1) != 0;
        reader.Skip(1); // Reserved
        var count = reader.ReadByte();
        
        Items = new SubPlayItem[count];
        for (var i = 0; i < count; i++)
        {
            var item = new SubPlayItem();
            item.Read(reader);
            Items[i] = item;
        }
        
        reader.SkipTo(start + length);
    }
}