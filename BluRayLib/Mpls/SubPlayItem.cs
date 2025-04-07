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
    
    public void Read(BigEndianBinaryReader reader)
    {
        var length = reader.ReadUInt16();
        var start = reader.Position;
        
        Name = reader.ReadString(5);
        Type = reader.ReadString(4);
        
        reader.SkipTo(start + length);
    }
}