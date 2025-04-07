using BluRayLib.Utils.IO;

namespace BluRayLib.Mpls;

public class MultiClipEntry
{
    /// <summary>
    /// Gets the item id.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Gets the file type.
    /// </summary>
    public string Type { get; set; } = "";
    
    public ushort StcId { get; set; }

    public void Read(BigEndianBinaryReader reader)
    {
        Name = reader.ReadString(5);
        Type = reader.ReadString(4);
        StcId = reader.ReadByte();
    }
}