using BluRayLib.Enums;
using MediaLib.Utils.IO;

namespace BluRayLib.Mpls;

public class StreamEntry
{
    public MplsStreamType Type { get; set; }
    public byte RefToSubPathId { get; set; }
    public byte RefToSubClipId { get; set; }
    public ushort RefToStreamId { get; set; }
    
    public void Read(BigEndianBinaryReader reader)
    {
        var length = reader.ReadByte();
        var start = reader.Position;
        if (length == 0) return;
        
        Type = (MplsStreamType)reader.ReadByte();
        switch (Type)
        {
            case MplsStreamType.PlayItem:
                RefToStreamId = reader.ReadUInt16();
                break;
            case MplsStreamType.SubPath1:
                RefToSubPathId = reader.ReadByte();
                RefToSubClipId = reader.ReadByte();
                RefToStreamId = reader.ReadUInt16();
                break;
            case MplsStreamType.SubPath2:
            case MplsStreamType.SubPath3:
                RefToSubPathId = reader.ReadByte();
                RefToStreamId = reader.ReadUInt16();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        reader.SkipTo(start + length);
    }
    
}