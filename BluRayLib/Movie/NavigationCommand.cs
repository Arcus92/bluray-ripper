using MediaLib.Utils.IO;

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
        var bits = reader.ReadBits8();
        OperationCount = bits.ReadBits(3);
        Group = bits.ReadBits(2);
        SupGroup = bits.ReadBits(3);
        
        bits = reader.ReadBits8();
        ImmOperation1 = bits.ReadBit();
        ImmOperation2 = bits.ReadBit();
        bits.Skip(2); // Skip 2 bits
        BranchOption = bits.ReadBits(4);
        
        bits = reader.ReadBits8();
        bits.Skip(4); // Skip 4 bits
        CompareOption = bits.ReadBits(4);
        
        bits = reader.ReadBits8();
        bits.Skip(3); // Skip 3 bits
        SetOption = bits.ReadBits(5);
        
        Destination = reader.ReadUInt32();
        Source = reader.ReadUInt32();
    }
}