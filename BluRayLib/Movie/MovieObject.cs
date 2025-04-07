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
        var value = reader.ReadUInt16();
        ResumeIntention = BitUtils.GetBitFromLeft(value, 0);
        MenuCallMask = BitUtils.GetBitFromLeft(value, 1);
        TitleSearchMask = BitUtils.GetBitFromLeft(value, 2);
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