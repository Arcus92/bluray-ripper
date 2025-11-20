using MediaLib.Utils.IO;

namespace DvdLib.Vmg;

public class MultiChannelAttributes
{
    public bool Ach0Gme { get; set; }
    public bool Ach1Gme { get; set; }

    public void Read(BigEndianBinaryReader reader)
    {
        var b = reader.ReadBits8();
        b.Skip(7);
        Ach0Gme = b.ReadBit();
        
        b = reader.ReadBits8();
        b.Skip(7);
        Ach1Gme = b.ReadBit();
        
        b = reader.ReadBits8();
        
        b = reader.ReadBits8();
        
        b = reader.ReadBits8();
        
        reader.Skip(19);
    }
    
    /// <summary>
    /// Reads the sub-picture attribute from the reader and returns the instance.
    /// </summary>
    /// <param name="reader">The current reader.</param>
    /// <returns>Returns the read instance.</returns>
    public static MultiChannelAttributes FromReader(BigEndianBinaryReader reader)
    {
        var attr = new MultiChannelAttributes();
        attr.Read(reader);
        return attr;
    }

    /// <summary>
    /// Reads the given number ob sub-picture attributes from the reader and returns the instances.
    /// </summary>
    /// <param name="reader">The current reader.</param>
    /// <param name="count">The number of items to read.</param>
    /// <returns>Returns the read instances.</returns>
    public static MultiChannelAttributes[] FromReader(BigEndianBinaryReader reader, int count)
    {
        var array = new MultiChannelAttributes[count];
        for (var i = 0; i < count; i++)
        {
            array[i] = FromReader(reader);
        }
        return array;
    }
}