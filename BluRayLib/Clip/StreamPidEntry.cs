using MediaLib.Utils.IO;

namespace BluRayLib.Clip;

public class StreamPidEntry
{
    public ushort Pid { get; set; }
    
    public byte EpStreamType { get; set; }

    public EpCoarseEntry[] EpCoarseEntries { get; set; } = [];

    public EpFineEntry[] EpFineEntries { get; set; } = [];
    
    public uint EpMapForOneStreamPidStart { get; set; }
    
    public void Read(BigEndianBinaryReader reader)
    {
        var bits = reader.ReadBits64();
        Pid = (ushort)bits.ReadBits(16);
        bits.Skip(10); // Skip 10 bits
        EpStreamType = (byte)bits.ReadBits(4);
        
        var epCoarseEntries = (ushort)bits.ReadBits(16);
        EpCoarseEntries = new EpCoarseEntry[epCoarseEntries];
        for (var i = 0; i < epCoarseEntries; i++)
        {
            EpCoarseEntries[i] = new EpCoarseEntry();
        }
        
        var epFineEntries = (uint)bits.ReadBits(18);
        EpFineEntries = new EpFineEntry[epFineEntries];
        for (var i = 0; i < epFineEntries; i++)
        {
            EpFineEntries[i] = new EpFineEntry();
        }
        
        EpMapForOneStreamPidStart = reader.ReadUInt32();
    }
}