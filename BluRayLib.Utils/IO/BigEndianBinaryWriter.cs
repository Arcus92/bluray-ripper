using System.Buffers.Binary;

namespace BluRayLib.Utils.IO;

/// <summary>
/// A variant of the <see cref="BinaryWriter"/> but in big-endian.
/// </summary>
public class BigEndianBinaryWriter : IDisposable
{
    public BigEndianBinaryWriter(Stream stream)
    {
        BaseStream = stream;
        _writer = new BinaryWriter(BaseStream);
    }
    
    /// <summary>
    /// Gets the base stream.
    /// </summary>
    public Stream BaseStream { get; }

    /// <summary>
    /// The little-endian writer.
    /// </summary>
    private readonly BinaryWriter _writer;
    
    /// <inheritdoc />
    public void Dispose()
    {
        BaseStream.Dispose();
    }

    /// <summary>
    /// Writes a 8-bit byte.
    /// </summary>
    /// <param name="value">The value.</param>
    public void Write(byte value)
    {
        _writer.Write(value);
    }

    /// <summary>
    /// Writes an unsigned 16-bit integer.
    /// </summary>
    /// <param name="value">The value.</param>
    public void Write(ushort value)
    {
        _writer.Write(BinaryPrimitives.ReverseEndianness(value));
    }
    
    /// <summary>
    /// Writes an 16-bit integer.
    /// </summary>
    /// <param name="value">The value.</param>
    public void Write(short value)
    {
        _writer.Write(BinaryPrimitives.ReverseEndianness(value));
    }
    
    /// <summary>
    /// Writes an unsigned 24-bit integer.
    /// </summary>
    /// <param name="value">The value.</param>
    public void WriteUInt24(int value)
    {
        Write((byte)((value >> 16) & 0xFF));
        Write((byte)((value >> 8) & 0xFF));
        Write((byte)(value & 0xFF));
    }
    
    /// <summary>
    /// Writes an unsigned 32-bit integer.
    /// </summary>
    /// <param name="value">The value.</param>
    public void Write(uint value)
    {
        _writer.Write(BinaryPrimitives.ReverseEndianness(value));
    }
    
    /// <summary>
    /// Writes an 32-bit integer.
    /// </summary>
    /// <param name="value">The value.</param>
    public void Write(int value)
    {
        _writer.Write(BinaryPrimitives.ReverseEndianness(value));
    }
    
    /// <summary>
    /// Writes an unsigned 64-bit integer.
    /// </summary>
    /// <param name="value">The value.</param>
    public void Write(ulong value)
    {
        _writer.Write(BinaryPrimitives.ReverseEndianness(value));
    }
    
    /// <summary>
    /// Writes an 64-bit integer.
    /// </summary>
    /// <param name="value">The value.</param>
    public void Write(long value)
    {
        _writer.Write(BinaryPrimitives.ReverseEndianness(value));
    }
    
    /// <summary>
    /// Writes the given bytes.
    /// </summary>
    /// <param name="data">The byte array.</param>
    public void Write(byte[] data)
    {
        _writer.Write(data);
    }
}