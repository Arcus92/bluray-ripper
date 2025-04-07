using BluRayLib.Utils;
using BluRayLib.Utils.IO;

namespace BluRayLib.Clip;

public class EpCoarseEntry
{
    public uint RefToEpFineId { get; set; }
    public ushort PtsEpCoarse { get; set; }
    public uint SpnEpCoarse { get; set; }

    public void Read(BigEndianBinaryReader reader)
    {
        var value = reader.ReadUInt64();
        RefToEpFineId = (uint)BitUtils.GetBitsFromLeft(value, 0, 18);
        PtsEpCoarse = (ushort)BitUtils.GetBitsFromLeft(value, 18, 14);
        SpnEpCoarse = (uint)BitUtils.GetBitsFromLeft(value, 32, 32);
    }
}