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
        var bits = reader.ReadBits32();
        ReservedEpFine = bits.ReadBit();
        EndPositionOffset = (byte)bits.ReadBits(3);
        PtsEpCoarse = (ushort)bits.ReadBits(11);
        SpnEpCoarse = bits.ReadBits(17);
    }
}