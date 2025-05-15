using MediaLib.Utils.IO;

namespace BluRayLib.PresentationGraphicStream;

/// <summary>
/// A Presentation Graphic Stream (PGS) from a .sup file.
/// </summary>
public class SupFilePresentationGraphicStream : IPresentationGraphicStream, IDisposable, IAsyncDisposable
{
    /// <summary>
    /// The stream to read the .sup file from.
    /// </summary>
    private readonly Stream _stream;

    /// <summary>
    /// Creates a .sup file PGS.
    /// </summary>
    /// <param name="filename">The file to read from.</param>
    public SupFilePresentationGraphicStream(string filename) : this(new FileStream(filename, FileMode.Open, FileAccess.Read))
    {
    }
    
    /// <summary>
    /// Creates a .sup file PGS.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    public SupFilePresentationGraphicStream(Stream stream)
    {
        _stream = stream;
    }
    
    /// <inheritdoc />
    public async IAsyncEnumerable<DisplaySet> ReadAsync()
    {
        var reader = new BigEndianBinaryReader(_stream);

        while (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            var displaySet = new DisplaySet();
            displaySet.Read(reader, includeHeader: true);
            yield return displaySet;
        }
    }
    
    #region IDisposable

    /// <inheritdoc />
    public void Dispose()
    {
        _stream.Dispose();
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await _stream.DisposeAsync();
    }
    
    #endregion IDisposable
}