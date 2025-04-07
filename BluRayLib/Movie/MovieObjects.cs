using BluRayLib.Utils.IO;

namespace BluRayLib.Movie;

public class MovieObjects
{
    /// <summary>
    /// Gets and sets the movie objects.
    /// </summary>
    public MovieObject[] Objects { get; set; } = [];
    
    /// <summary>
    /// Reads the movie-objects file.
    /// </summary>
    /// <param name="path">The path to the playlist file.</param>
    public void Read(string path)
    {
        using var fileStream = File.OpenRead(path);
        Read(fileStream);
    }
    
    /// <summary>
    /// Reads the movie-objects file from stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    public void Read(Stream stream)
    {
        var reader = new BigEndianBinaryReader(stream);
        Read(reader);
    }

    private void Read(BigEndianBinaryReader reader)
    {
        var length = reader.ReadUInt32();
        var start = reader.Position;
        
        reader.Skip(32);
        var count = reader.ReadUInt16();
        Objects = new MovieObject[count];
        for (var i = 0; i < count; i++)
        {
            var obj = new MovieObject();
            obj.Read(reader);
            Objects[i] = obj;
        }
        
        reader.SkipTo(start + length);
    }
}