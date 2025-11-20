namespace DvdLib.Decrypt;

public class DvdCssDecryptStream : Stream
{
    private readonly DvdCss _dvdCss;
    private readonly long _length;
    
    public DvdCssDecryptStream(string filename)
    {
        _dvdCss = new DvdCss();
        var fileInfo = new FileInfo(filename);
        _length = fileInfo.Length;
        
        if (!_dvdCss.Open(filename) || !_dvdCss.Seek(0, DvdCssSeekFlags.Key))
        {
            throw new IOException($"DvdCss couldn't open disk path: {filename}");
        }
    }

    private const int BlocksInBuffer = 32;
    private const int BufferSize = DvdCss.BlockSize * BlocksInBuffer;
    private long _fileOffset;
    private int _bufferOffset;
    private readonly byte[] _buffer = new byte[BufferSize];
    private void ReadBufferAndDecrypt()
    {
        _dvdCss.Read(_buffer, BlocksInBuffer, DvdCssReadFlags.Decrypt);
    }
    
    /// <inheritdoc />
    public override void Flush()
    {
    }

    /// <inheritdoc />
    public override int Read(byte[] buffer, int offset, int count)
    {
        var readTotal = 0;
        while (count > 0)
        {
            if (_bufferOffset == 0)
            {
                // Check end of file
                if (_fileOffset >= _length)
                    break;
                ReadBufferAndDecrypt();
            }

            // Calculate the bytes to read in this chunk
            var read = BufferSize - _bufferOffset;
            if (read > count) read = count;

            Buffer.BlockCopy(_buffer, _bufferOffset, buffer, offset, read);

            _bufferOffset += read;
            readTotal += read;
            offset += read;
            count -= read;
            
            // Jump to next unit
            if (_bufferOffset == BufferSize)
            {
                _fileOffset += BufferSize;
                _bufferOffset = 0;
            }
        }

        return readTotal;
    }

    /// <inheritdoc />
    public override long Seek(long offset, SeekOrigin origin)
    {
        // Ignore origin and convert to begin-position
        offset = origin switch
        {
            SeekOrigin.Current => Position + offset,
            SeekOrigin.End => Length + offset,
            _ => offset
        };


        var newBufferOffset = (int)(offset % BufferSize);
        var newFileOffset = offset - newBufferOffset;
            
        // Unit is already loaded
        if (newFileOffset == _fileOffset && _bufferOffset > 0)
        {
            _bufferOffset = newBufferOffset;
            return offset;
        }

        if (!_dvdCss.Seek((int)(newFileOffset / BufferSize), DvdCssSeekFlags.Key))
        {
            throw new IOException("Failed to seek DvdCss stream.");
        }
        
        _fileOffset = newFileOffset;
        _bufferOffset = newBufferOffset;
        if (_bufferOffset != 0)
            ReadBufferAndDecrypt();
        return offset;
    }

    /// <inheritdoc />
    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public override bool CanRead => true;

    /// <inheritdoc />
    public override bool CanSeek => true;

    /// <inheritdoc />
    public override bool CanWrite => false;

    /// <inheritdoc />
    public override long Length => _length;

    /// <inheritdoc />
    public override long Position
    {
        get => _fileOffset + _bufferOffset;
        set => Seek(value, SeekOrigin.Begin);
    }
    
    #region Static

    /// <summary>
    /// Opens a filename from a DVD.
    /// </summary>
    /// <param name="diskPath">The path to the DVD root directory.</param>
    /// <param name="filename">The filename to the file.</param>
    /// <returns></returns>
    public static DvdCssDecryptStream Open(string diskPath, string filename)
    {
        var inputPath = Path.Combine(diskPath, filename);
        var stream = new DvdCssDecryptStream(inputPath);
        return stream;
    }
    
    #endregion Static
    
    #region IDisposable
    

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _dvdCss.Dispose();
    }
    
    #endregion IDisposable
}