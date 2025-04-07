using BluRayLib.Utils;
using BluRayLib.Utils.IO;

namespace BluRayLib.Movie;

public class NavigationCommand
{
    public byte OperationCount { get; set; }
    public byte Group { get; set; }
    public byte SupGroup { get; set; }
    public bool ImmOperation1 { get; set; }
    public bool ImmOperation2 { get; set; }
    public byte BranchOption { get; set; }
    public byte CompareOption { get; set; }
    public byte SetOption { get; set; }
    public uint Destination { get; set; }
    public uint Source { get; set; }
    
    public void Read(BigEndianBinaryReader reader)
    {
        var value = reader.ReadByte();
        OperationCount = BitUtils.GetBitsFromLeft(value, 0, 3);
        Group = BitUtils.GetBitsFromLeft(value, 3, 2);
        SupGroup = BitUtils.GetBitsFromLeft(value, 5, 3);
        
        value = reader.ReadByte();
        ImmOperation1 = BitUtils.GetBitFromLeft(value, 0);
        ImmOperation2 = BitUtils.GetBitFromLeft(value, 1);
        // Skip 2 bits
        BranchOption = BitUtils.GetBitsFromLeft(value, 4, 4);
        
        value = reader.ReadByte();
        // Skip 4 bits
        CompareOption = BitUtils.GetBitsFromLeft(value, 4, 4);
        
        value = reader.ReadByte();
        // Skip 3 bits
        SetOption = BitUtils.GetBitsFromLeft(value, 3, 5);
        
        Destination = reader.ReadUInt32();
        Source = reader.ReadUInt32();
    }
}