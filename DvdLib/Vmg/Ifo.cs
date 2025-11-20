using MediaLib.Utils.IO;

namespace DvdLib.Vmg;

public class Ifo
{
    // https://dvds.beandog.org/doku.php?id=libdvdread
    // https://code.videolan.org/videolan/libdvdread/-/blob/master/src/ifo_read.c?ref_type=heads
    // https://code.videolan.org/videolan/libdvdread/-/blob/master/src/dvdread/ifo_types.h?ref_type=heads

    public AudioAttributes[] AudioList { get; set; } = [];
    public SubPictureAttributes[] SubPictureList { get; set; } = [];

    /// <summary>
    /// Reads the IFO file.
    /// </summary>
    /// <param name="path">The path to the IFO file.</param>
    public void Read(string path)
    {
        using var fileStream = File.OpenRead(path);
        Read(fileStream);
    }
    
    /// <summary>
    /// Reads the IFO file from stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    public void Read(Stream stream)
    {
        var reader = new BigEndianBinaryReader(stream);
        Read(reader);
    }

    private void Read(BigEndianBinaryReader reader)
    {
        var header = reader.ReadString(12);
        switch (header)
        {
            case "DVDVIDEO-VMG":
                ReadVmg(reader);
                break;
            case "DVDVIDEO-VTS":
                ReadVts(reader);
                break;
            default:
                throw new InvalidDataException("Invalid IFO magic number!");
        }
    }

    private void ReadVts(BigEndianBinaryReader reader)
    {
        var vtsLastSector = reader.ReadUInt32();
        reader.Skip(12);
        var vtsiLastSector = reader.ReadUInt32();
        reader.Skip(1);

        var specificationVersion = reader.ReadByte();
        var vtsCategory = reader.ReadUInt32();
        reader.Skip(2);
        reader.Skip(2);
        reader.Skip(1);
        reader.Skip(19);
        reader.Skip(2);
        reader.Skip(32);
        reader.Skip(8);
        reader.Skip(24);
        var vtsiLastByte = reader.ReadUInt32();
        reader.Skip(4);
        reader.Skip(56);
        var vtsmVobs =  reader.ReadUInt32();
        var vtsttVobs =  reader.ReadUInt32();
        var vtsPttVobs =  reader.ReadUInt32();
        var vtsPgcit =  reader.ReadUInt32();
        var vtsmPgciUt =  reader.ReadUInt32();
        var vtsTmapt =  reader.ReadUInt32();
        var vtsmCAdt =  reader.ReadUInt32();
        var vtsmVobuAdmap =  reader.ReadUInt32();
        var vtsCAdt =  reader.ReadUInt32();
        var vtsVobuAdmap =  reader.ReadUInt32();
        reader.Skip(24);
        var videoAttrT = VideoAttributes.FromReader(reader);
        reader.Skip(1);
        var nrOfVtsmAudioStreams = reader.ReadByte();
        var vtsmAudioAttr = AudioAttributes.FromReader(reader);
        reader.Skip(8 * 7);
        reader.Skip(17);
        var nrOfVtsmSubpStreams = reader.ReadByte();
        var vtsmSubpAttr = reader.ReadUInt40();
        reader.Skip(6 * 27);
        reader.Skip(2);
        var vtsVideoAttr = reader.ReadUInt16();
        reader.Skip(1);
        var nrOfVtsAudioAttr = reader.ReadByte();
        var vtsAudioAttr = AudioAttributes.FromReader(reader, 8);
        AudioList = vtsAudioAttr.AsSpan(0, nrOfVtsAudioAttr).ToArray();
        reader.Skip(17);
        var nrOfVtsSubPictureStreams = reader.ReadByte();
        var vtsSubPictureAttr = SubPictureAttributes.FromReader(reader, 32);
        SubPictureList = vtsSubPictureAttr.AsSpan(0, nrOfVtsSubPictureStreams).ToArray();
        reader.Skip(2);
        var vtsMuAudioAttr = MultiChannelAttributes.FromReader(reader, 8);
    }

    private void ReadVmg(BigEndianBinaryReader reader)
    {
        
    }
}