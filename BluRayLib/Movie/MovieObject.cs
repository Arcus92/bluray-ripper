using BluRayLib.Utils;
using BluRayLib.Utils.IO;

namespace BluRayLib.Movie;

public class MovieObject
{
    public bool ResumeIntention { get; set; }
    public bool MenuCallMask { get; set; }
    public bool TitleSearchMask { get; set; }
    public NavigationCommand[] Commands { get; set; } = [];

    public void Read(BigEndianBinaryReader reader)
    {
        var bits = reader.ReadBits16();
        ResumeIntention = bits.ReadBit();
        MenuCallMask = bits.ReadBit();
        TitleSearchMask = bits.ReadBit();
        // Skip 13 bits
        var count = reader.ReadUInt16();
        Commands = new NavigationCommand[count];
        for (var i = 0; i < count; i++)
        {
            var command = new NavigationCommand();
            command.Read(reader);
            Commands[i] = command;
        }
    }
}