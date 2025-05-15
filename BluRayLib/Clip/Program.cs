using MediaLib.Utils.IO;

namespace BluRayLib.Clip;

public class Program
{
    public uint SpnProgramSequenceStart { get; set; }
    public ushort ProgramMapPid { get; set; }

    public ProgramStream[] Streams { get; set; } = [];
    public void Read(BigEndianBinaryReader reader)
    {
        SpnProgramSequenceStart = reader.ReadUInt32();
        ProgramMapPid = reader.ReadUInt16();
        var count = reader.ReadByte();
        reader.Skip(1);
        Streams = new ProgramStream[count];
        for (var i = 0; i < count; i++)
        {
            var streamInfo = new ProgramStream();
            streamInfo.Read(reader);
            Streams[i] = streamInfo;
        }
    }
}