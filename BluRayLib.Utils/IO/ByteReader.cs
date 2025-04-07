namespace BluRayLib.Utils.IO;

/// <summary>
/// A simple byte array reader.
/// </summary>
public ref struct ByteReader
{
    /// <summary>
    /// The data span to read.
    /// </summary>
    private readonly ReadOnlySpan<byte> _data;
    
    /// <summary>
    /// The current position in the data.
    /// </summary>
    private int _position;
    
    /// <summary>
    /// Gets the current position in the data
    /// </summary>
    public int Position => _position;
    
    /// <summary>
    /// Gets the length of the data stream.
    /// </summary>
    public int Length => _data.Length;
    
    public ByteReader(ReadOnlySpan<byte> data)
    {
        _data = data;
    }
    
    /// <summary>
    /// Reads a single byte from the data.
    /// </summary>
    /// <returns></returns>
    public byte ReadByte()
    {
        return _data[_position++];
    }
    
    /// <summary>
    /// Reads an 16-bit integer.
    /// </summary>
    /// <returns></returns>
    public short ReadInt16()
    {
        var b1 = ReadByte();
        var b2 = ReadByte();
        return (short)((b1 << 8) + b2);
    }
    
    /// <summary>
    /// Reads an 16-bit unsigned integer.
    /// </summary>
    /// <returns></returns>
    public ushort ReadUInt16()
    {
        var b1 = ReadByte();
        var b2 = ReadByte();
        return (ushort)((b1 << 8) + b2);
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
        var b1 = ReadByte();
        var b2 = ReadByte();
        var b3 = ReadByte();
        var b4 = ReadByte();
        return (b1 << 24) + (b2 << 16) + (b3 << 8) + b4;
    }
    
    /// <summary>
    /// Reads an 32-bit unsigned integer.
    /// </summary>
    /// <returns></returns>
    public uint ReadUInt32()
    {
        var b1 = ReadByte();
        var b2 = ReadByte();
        var b3 = ReadByte();
        var b4 = ReadByte();
        return (uint)((b1 << 24) + (b2 << 16) + (b3 << 8) + b4);
    }

    
    /// <summary>
    /// Reads the number of bytes and returns a byte span.
    /// </summary>
    /// <param name="bytes">The number of bytes to read.</param>
    /// <returns></returns>
    public ReadOnlySpan<byte> Read(int bytes)
    {
        var data = _data.Slice(_position, bytes);
        _position += bytes;
        return data;
    }
    
    /// <summary>
    /// Moves the position in the stream forward without reading the data.
    /// </summary>
    /// <param name="bytes">Number of bytes to skip.</param>
    public void Skip(int bytes)
    {
        _position += bytes;
    }
}