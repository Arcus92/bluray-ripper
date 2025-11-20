using MediaLib.Utils.IO;

namespace DvdLib.Vmg;

public class DvdTime
{
    public byte Hour { get; set; }
    public byte Minute { get; set; }
    public byte Second { get; set; }
    public byte Frame { get; set; }

    public void Read(BigEndianBinaryReader reader)
    {
        Hour = reader.ReadByte();
        Minute = reader.ReadByte();
        Second = reader.ReadByte();
        Frame = reader.ReadByte();
    }
}