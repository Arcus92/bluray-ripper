using System.Buffers.Binary;
using System.Text;

namespace BluRayLib.Utils.IO;

/// <summary>
/// A variant of the <see cref="BinaryReader"/> but in big-endian.
/// </summary>
public class BigEndianBinaryReader : IDisposable
{
    public BigEndianBinaryReader(Stream stream)
    {
        BaseStream = stream;
        _reader = new BinaryReader(BaseStream);
        _position = _reader.BaseStream.Position;
    }
    
    public BigEndianBinaryReader(byte[] data) : this(new MemoryStream(data))
    {
    }

    /// <summary>
    /// Gets the base stream.
    /// </summary>
    public Stream BaseStream { get; }

    /// <summary>
    /// The little-endian reader.
    /// </summary>
    private readonly BinaryReader _reader;

    /// <summary>
    /// The current position.
    /// </summary>
    private long _position;

    

    /// <summary>
    /// Gets and sets the byte position in the given stream.
    /// </summary>
    public long Position
    {
        get => _position;
        set
        {
            if (_position == value) return;
            _position = value;
            BaseStream.Position = value;
        }
    }

    /// <summary>
    /// Gets the length of the buffer to read.
    /// </summary>
    public long Length => BaseStream.Length;
    
    /// <summary>
    /// Gets the number of available bytes.
    /// </summary>
    public long Available => Length - Position;
    
    /// <inheritdoc />
    public void Dispose()
    {
        BaseStream.Dispose();
    }
    
    /// <summary>
    /// Reads an 8-bit byte.
    /// </summary>
    /// <returns></returns>
    public byte ReadByte()
    {
        _position++;
        return _reader.ReadByte();
    }
    
    /// <summary>
    /// Reads an 16-bit integer.
    /// </summary>
    /// <returns></returns>
    public short ReadInt16()
    {
        _position += sizeof(short);
        return BinaryPrimitives.ReverseEndianness(_reader.ReadInt16());
    }
    
    /// <summary>
    /// Reads an 16-bit unsigned integer.
    /// </summary>
    /// <returns></returns>
    public ushort ReadUInt16()
    {
        _position += sizeof(ushort);
        return BinaryPrimitives.ReverseEndianness(_reader.ReadUInt16());
    }
    
    /// <summary>
    /// Reads an 24-bit unsigned integer.
    /// </summary>
    /// <returns></returns>
    public int ReadUInt24()
    {
        var b1 = ReadByte();
        var b2 = ReadByte();
        var b3 = ReadByte();
        return (b1 << 16) + (b2 << 8) + b3;
    }
    
    /// <summary>
    /// Reads an 32-bit integer.
    /// </summary>
    /// <returns></returns>
    public int ReadInt32()
    {
        _position += sizeof(int);
        return BinaryPrimitives.ReverseEndianness(_reader.ReadInt32());
    }
    
    /// <summary>
    /// Reads an 32-bit unsigned integer.
    /// </summary>
    /// <returns></returns>
    public uint ReadUInt32()
    {
        _position += sizeof(uint);
        return BinaryPrimitives.ReverseEndianness(_reader.ReadUInt32());
    }
    
    /// <summary>
    /// Reads an 64-bit integer.
    /// </summary>
    /// <returns></returns>
    public long ReadInt64()
    {
        _position += sizeof(long);
        return BinaryPrimitives.ReverseEndianness(_reader.ReadInt64());
    }
    
    /// <summary>
    /// Reads an 64-bit unsigned integer.
    /// </summary>
    /// <returns></returns>
    public ulong ReadUInt64()
    {
        _position += sizeof(ulong);
        return BinaryPrimitives.ReverseEndianness(_reader.ReadUInt64());
    }

    /// <summary>
    /// Reads the number of bytes.
    /// </summary>
    /// <param name="count">The number of bytes to read.</param>
    /// <returns></returns>
    public byte[] ReadBytes(int count)
    {
        _position += count;
        return _reader.ReadBytes(count);
    }

    /// <summary>
    /// Reads a fixed sized UTF8 string.
    /// </summary>
    /// <param name="length">The length of the string in bytes.</param>
    /// <returns>Returns the string.</returns>
    public string ReadString(int length)
    {
        var bytes = ReadBytes(length);
        return Encoding.UTF8.GetString(bytes);
    }

    /// <summary>
    /// Skips the next number of bytes.
    /// </summary>
    /// <param name="count">The bytes to skip.</param>
    public void Skip(long count)
    {
        Position += count;
    }

    /// <summary>
    /// Skips to the given position.
    /// Fails if the position was already past.
    /// </summary>
    /// <param name="position">The target position.</param>
    public void SkipTo(long position)
    {
        var diff = position - Position;
        if (diff == 0) return;
        if (diff < 0) throw new ArgumentException();
        Skip(diff);
    }

    /// <summary>
    /// Returns a sub stream with the given length.
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public BigEndianBinaryReader SubReader(long length)
    {
        var data = ReadBytes((int)length); // TODO: Make this more efficient
        var memStream = new MemoryStream(data);
        return new BigEndianBinaryReader(memStream);
    }
}