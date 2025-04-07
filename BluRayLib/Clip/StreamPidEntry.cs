using BluRayLib.Utils;
using BluRayLib.Utils.IO;

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
        var value = reader.ReadUInt64();
        Pid = (ushort)BitUtils.GetBitsFromLeft(value, 0, 16);
        // Skip 10 bits
        EpStreamType = (byte)BitUtils.GetBitsFromLeft(value, 26, 4);
        
        var epCoarseEntries = (ushort)BitUtils.GetBitsFromLeft(value, 30, 16);
        EpCoarseEntries = new EpCoarseEntry[epCoarseEntries];
        for (var i = 0; i < epCoarseEntries; i++)
        {
            EpCoarseEntries[i] = new EpCoarseEntry();
        }
        
        var epFineEntries = (uint)BitUtils.GetBitsFromLeft(value, 46, 18);
        EpFineEntries = new EpFineEntry[epFineEntries];
        for (var i = 0; i < epFineEntries; i++)
        {
            EpFineEntries[i] = new EpFineEntry();
        }
        
        EpMapForOneStreamPidStart = reader.ReadUInt32();
    }
}