using BluRayLib.Enums;
using MediaLib.Utils.IO;

namespace BluRayLib.Mpls;

public class PlaylistMark
{
    /// <summary>
    /// Gets and sets the marker type.
    /// </summary>
    public MarkType Type { get; set; }
    
    /// <summary>
    /// Gets and sets the play item id
    /// </summary>
    public ushort PlayItemId { get; set; }
    
    /// <summary>
    /// Gets and sets the timestamp in 45000ths of a seconds.
    /// </summary>
    public uint TimeStamp { get; set; }
    
    /// <summary>
    /// Exact purpose unknown
    /// </summary>
    public ushort ESPID { get; set; }
    
    /// <summary>
    /// Gets and sets the duration in 45000ths of a seconds.
    /// </summary>
    public uint Duration { get; set; }
    
    public void Read(BigEndianBinaryReader reader)
    {
        reader.Skip(1); // Unused
        Type = (MarkType)reader.ReadByte();

        PlayItemId = reader.ReadUInt16();
        TimeStamp = reader.ReadUInt32();
        ESPID = reader.ReadUInt16();
        Duration = reader.ReadUInt32();
    }
}