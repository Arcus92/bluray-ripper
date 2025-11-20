using MediaLib.Utils.IO;

namespace DvdLib.Vmg;

public class VideoAttributes
{
    public MpegVersion MpegVersion { get; set; }
    public VideoFormat VideoFormat { get; set; }
    public DisplayAspectRatio DisplayAspectRatio { get; set; }
    public byte PermittedDf { get; set; }

    public bool Line21Cc1 { get; set; }
    public bool Line21Cc2 { get; set; }
    public bool BitRate { get; set; }
    
    public PictureSize PictureSize { get; set; }
    public bool LetterBoxed { get; set; }
    public FilmMode FilmMode { get; set; }

    public void Read(BigEndianBinaryReader reader)
    {
        var b = reader.ReadBits8();
        MpegVersion = (MpegVersion)b.ReadBits(2);
        VideoFormat = (VideoFormat)b.ReadBits(2);
        DisplayAspectRatio = (DisplayAspectRatio)b.ReadBits(2);
        PermittedDf = b.ReadBits(2);
        
        b = reader.ReadBits8();
        Line21Cc1 = b.ReadBit();
        Line21Cc2 = b.ReadBit();
        b.Skip(1);
        BitRate = b.ReadBit();
        
        b = reader.ReadBits8();
        PictureSize =  (PictureSize)b.ReadBits(2);
        LetterBoxed = b.ReadBit();
        FilmMode = (FilmMode)b.ReadBits(1);
    }
    
    /// <summary>
    /// Reads the video attribute from the reader and returns the instance.
    /// </summary>
    /// <param name="reader">The current reader.</param>
    /// <returns>Returns the read instance.</returns>
    public static VideoAttributes FromReader(BigEndianBinaryReader reader)
    {
        var attr = new VideoAttributes();
        attr.Read(reader);
        return attr;
    }

    /// <summary>
    /// Reads the given number ob video attributes from the reader and returns the instances.
    /// </summary>
    /// <param name="reader">The current reader.</param>
    /// <param name="count">The number of items to read.</param>
    /// <returns>Returns the read instances.</returns>
    public static VideoAttributes[] FromReader(BigEndianBinaryReader reader, int count)
    {
        var array = new VideoAttributes[count];
        for (var i = 0; i < count; i++)
        {
            array[i] = FromReader(reader);
        }
        return array;
    }
}