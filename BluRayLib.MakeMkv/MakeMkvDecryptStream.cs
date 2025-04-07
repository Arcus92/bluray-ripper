namespace MakeMkvLib;

/// <summary>
/// A decryption stream to read an encrypted BluRay file. For example an M2TS file.
/// </summary>
public class MakeMkvDecryptStream : Stream
{
    private readonly MakeMkv _makeMkv;
    private readonly FileNameFlags _nameFlags;
    private readonly Stream _inputStream;
    
    public MakeMkvDecryptStream(MakeMkv makeMkv, FileNameFlags nameFlags, Stream inputStream)
    {
        _makeMkv = makeMkv;
        _nameFlags = nameFlags;
        _inputStream = inputStream;
    }

    private long _fileOffset;
    private int _bufferOffset;
    private readonly byte[] _buffer = new byte[MakeMkv.UnitSize];
    private void ReadBufferAndDecrypt()
    {
        _inputStream.ReadExactly(_buffer, 0, _buffer.Length);
        _makeMkv.DecryptUnit(_nameFlags, (ulong)_fileOffset, _buffer);
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
                if (_inputStream.Position == _inputStream.Length)
                    break;
                ReadBufferAndDecrypt();
            }

            // Calculate the bytes to read in this chunk
            var read = MakeMkv.UnitSize - _bufferOffset;
            if (read > count) read = count;

            Buffer.BlockCopy(_buffer, _bufferOffset, buffer, offset, read);

            _bufferOffset += read;
            readTotal += read;
            offset += read;
            count -= read;
            
            // Jump to next unit
            if (_bufferOffset == MakeMkv.UnitSize)
            {
                _fileOffset += MakeMkv.UnitSize;
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


        var newBufferOffset = (int)(offset % MakeMkv.UnitSize);
        var newFileOffset = offset - newBufferOffset;
            
        // Unit is already loaded
        if (newFileOffset == _fileOffset && _bufferOffset > 0)
        {
            _bufferOffset = newBufferOffset;
            return offset;
        }
        
        _fileOffset = newFileOffset;
        _bufferOffset = newBufferOffset;

        _inputStream.Seek(_fileOffset, SeekOrigin.Begin);
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
    public override bool CanSeek => _inputStream.CanSeek;

    /// <inheritdoc />
    public override bool CanWrite => false;

    /// <inheritdoc />
    public override long Length => _inputStream.Length;

    /// <inheritdoc />
    public override long Position
    {
        get => _fileOffset + _bufferOffset;
        set => Seek(value, SeekOrigin.Begin);
    }
    
    #region Static

    /// <summary>
    /// Opens a filename from a BluRay.
    /// </summary>
    /// <param name="diskPath">The path to the BluRay root directory.</param>
    /// <param name="nameFlags">The filename flags to the file. Use <c>FileName.M2TS(01000)</c> for example.</param>
    /// <returns></returns>
    public static MakeMkvDecryptStream Open(string diskPath, FileNameFlags nameFlags)
    {
        var makeMkv = new MakeMkv();
        makeMkv.Open(diskPath);
        var inputPath = Path.Combine(diskPath, nameFlags.GetLocalPath());
        var inputStream = File.OpenRead(inputPath);
        var stream = new MakeMkvDecryptStream(makeMkv, nameFlags, inputStream)
        {
            DisposeMakeMkv = true,
            DisposeInputStream = true
        };
        return stream;
    }
    
    #endregion Static
    
    #region IDisposable
    
    /// <summary>
    /// Gets and sets if the internal MakeMkv context should be destroyed on dispose.
    /// </summary>
    public bool DisposeMakeMkv { get; set; }
    
    /// <summary>
    /// Gets and sets if the input stream should be destroyed on dispose.
    /// </summary>
    public bool DisposeInputStream { get; set; }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        
        if (DisposeMakeMkv)
            _makeMkv.Dispose();
        if (DisposeInputStream)
            _inputStream.Dispose();
    }
    
    #endregion IDisposable
}