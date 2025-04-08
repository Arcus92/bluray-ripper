using System.Numerics;

namespace BluRayLib.Utils.IO;

/// <summary>
/// A big-endian bit reader. Starts at the left bit.
/// </summary>
/// <typeparam name="T"></typeparam>
public unsafe ref struct BigEndianBitReader<T> where T : unmanaged, IBinaryInteger<T>
{
    /// <summary>
    /// The binary value.
    /// </summary>
    private readonly T _value;
    
    /// <summary>
    /// The current bit position. Runs backwards for big endian.
    /// </summary>
    private int _position = Size - 1;
    
    /// <summary>
    /// Gets the number of total bits of <see cref="T"/>.
    /// </summary>
    private static readonly int Size = sizeof(T) * 8;
    
    /// <summary>
    /// Gets the number of available bits.
    /// </summary>
    public int Available => _position;

    public BigEndianBitReader(T value)
    {
        _value = value;
    }

    /// <summary>
    /// Reads the next bit.
    /// </summary>
    /// <returns></returns>
    public bool ReadBit()
    {
        return (_value & (T.One << _position--)) != T.Zero;
    }

    /// <summary>
    /// Reads the next bits.
    /// </summary>
    /// <param name="count">The number of bits to read.</param>
    /// <returns>The value.</returns>
    public T ReadBits(int count)
    {
        var value = (_value << (Size - _position - 1)) >> (Size - count);
        _position -= count;
        return value;
    }

    /// <summary>
    /// Skips the number of bits.
    /// </summary>
    /// <param name="count">The number of bits to skip.</param>
    public void Skip(int count)
    {
        _position -= count;
    }
}