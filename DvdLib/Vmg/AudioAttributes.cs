using MediaLib.Utils.IO;

namespace DvdLib.Vmg;

public class AudioAttributes
{
    public AudioFormat AudioFormat { get; set; }
    public bool MultichannelExtension { get; set; }
    public byte LandType { get; set; }
    public ApplicationMode ApplicationMode { get; set; }
    
    public byte Quantization { get; set; }
    public byte SampleFrequency { get; set; }
    public byte Channels { get; set; }


    public string LangCode { get; set; } = "";
    public byte LangExtension { get; set; }
    public CodeExtension CodeExtension { get; set; }
    
    public void Read(BigEndianBinaryReader reader)
    {
        var b = reader.ReadBits8();
        AudioFormat = (AudioFormat)b.ReadBits(3);
        MultichannelExtension = b.ReadBit();
        LandType = b.ReadBits(2);
        ApplicationMode = (ApplicationMode)b.ReadBits(2);
        
        b = reader.ReadBits8();
        Quantization = b.ReadBits(2);
        SampleFrequency = b.ReadBits(2);
        b.Skip(1);
        Channels = b.ReadBits(3);
        
        LangCode = reader.ReadString(2);
        LangExtension = reader.ReadByte();
        CodeExtension = (CodeExtension)reader.ReadByte();
        reader.Skip(1);
        
        b = reader.ReadBits8();
    }

    /// <summary>
    /// Reads the audio attribute from the reader and returns the instance.
    /// </summary>
    /// <param name="reader">The current reader.</param>
    /// <returns>Returns the read instance.</returns>
    public static AudioAttributes FromReader(BigEndianBinaryReader reader)
    {
        var attr = new AudioAttributes();
        attr.Read(reader);
        return attr;
    }

    /// <summary>
    /// Reads the given number ob audio attributes from the reader and returns the instances.
    /// </summary>
    /// <param name="reader">The current reader.</param>
    /// <param name="count">The number of items to read.</param>
    /// <returns>Returns the read instances.</returns>
    public static AudioAttributes[] FromReader(BigEndianBinaryReader reader, int count)
    {
        var array = new AudioAttributes[count];
        for (var i = 0; i < count; i++)
        {
            array[i] = FromReader(reader);
        }
        return array;
    }
}