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
        var bits = reader.ReadBits64();
        RefToEpFineId = (uint)bits.ReadBits(18);
        PtsEpCoarse = (ushort)bits.ReadBits(14);
        SpnEpCoarse = (uint)bits.ReadBits(32);
    }
}