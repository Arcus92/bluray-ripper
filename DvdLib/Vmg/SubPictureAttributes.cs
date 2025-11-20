using MediaLib.Utils.IO;

namespace DvdLib.Vmg;

public class SubPictureAttributes
{
    public CodingMode CodingMode { get; set; }
    public LanguageType Type { get; set; }
    public string LangCode { get; set; } = "";
    public byte LangExtension { get; set; }
    public CodeExtension CodeExtension { get; set; }

    public void Read(BigEndianBinaryReader reader)
    {
        var b = reader.ReadBits8();
        CodingMode = (CodingMode)b.ReadBits(3);
        b.Skip(3);
        Type = (LanguageType)b.ReadBits(2);

        reader.Skip(1);
        LangCode = reader.ReadString(2);
        LangExtension = reader.ReadByte();
        CodeExtension = (CodeExtension)reader.ReadByte();
    }
    
    /// <summary>
    /// Reads the sub-picture attribute from the reader and returns the instance.
    /// </summary>
    /// <param name="reader">The current reader.</param>
    /// <returns>Returns the read instance.</returns>
    public static SubPictureAttributes FromReader(BigEndianBinaryReader reader)
    {
        var attr = new SubPictureAttributes();
        attr.Read(reader);
        return attr;
    }

    /// <summary>
    /// Reads the given number ob sub-picture attributes from the reader and returns the instances.
    /// </summary>
    /// <param name="reader">The current reader.</param>
    /// <param name="count">The number of items to read.</param>
    /// <returns>Returns the read instances.</returns>
    public static SubPictureAttributes[] FromReader(BigEndianBinaryReader reader, int count)
    {
        var array = new SubPictureAttributes[count];
        for (var i = 0; i < count; i++)
        {
            array[i] = FromReader(reader);
        }
        return array;
    }
}