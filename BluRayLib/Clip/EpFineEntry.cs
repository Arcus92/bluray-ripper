using BluRayLib.Utils;
using BluRayLib.Utils.IO;

namespace BluRayLib.Clip;

public class EpFineEntry
{
    public bool ReservedEpFine { get; set; }
    public byte EndPositionOffset { get; set; }
    public ushort PtsEpCoarse { get; set; }
    public uint SpnEpCoarse { get; set; }

    public void Read(BigEndianBinaryReader reader)
    {
        var value = reader.ReadUInt32();
        ReservedEpFine = BitUtils.GetBitFromLeft(value, 0);
        EndPositionOffset = (byte)BitUtils.GetBitsFromLeft(value, 1, 3);
        PtsEpCoarse = (ushort)BitUtils.GetBitsFromLeft(value, 4, 11);
        SpnEpCoarse = BitUtils.GetBitsFromLeft(value, 15, 17);
    }
}