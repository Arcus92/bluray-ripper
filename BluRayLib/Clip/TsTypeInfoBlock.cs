using BluRayLib.Utils.IO;

namespace BluRayLib.Clip;

public class TsTypeInfoBlock
{
    public void Read(BigEndianBinaryReader reader)
    {
        var length = reader.ReadUInt16();
        var start = reader.Position;
        
        
        
        reader.SkipTo(start + length);
    }
}