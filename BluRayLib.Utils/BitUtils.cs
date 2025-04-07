namespace BluRayLib.Utils;

public static class BitUtils
{
    /// <summary>
    /// Gets the bit at the given index.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="index">The bit index.</param>
    /// <returns>Returns if the bit is set to 1.</returns>
    public static bool GetBit(uint value, int index)
    {
        return (value & (1u << index)) != 0;
    }
    
    /// <summary>
    /// Gets the bit at the given index.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="index">The bit index.</param>
    /// <returns>Returns if the bit is set to 1.</returns>
    public static bool GetBit(ulong value, int index)
    {
        return (value & (1ul << index)) != 0;
    }
    
    /// <summary>
    /// Gets the bit at the given index from the left.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="index">The bit index.</param>
    /// <returns>Returns if the bit is set to 1.</returns>
    public static bool GetBitFromLeft(byte value, int index)
    {
        return (value & (1u << (7 - index))) != 0;
    }
    
    /// <summary>
    /// Gets the bit at the given index from the left.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="index">The bit index.</param>
    /// <returns>Returns if the bit is set to 1.</returns>
    public static bool GetBitFromLeft(ushort value, int index)
    {
        return (value & (1u << (15 - index))) != 0;
    }
    
    /// <summary>
    /// Gets the bit at the given index from the left.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="index">The bit index.</param>
    /// <returns>Returns if the bit is set to 1.</returns>
    public static bool GetBitFromLeft(uint value, int index)
    {
        return (value & (1u << (31 - index))) != 0;
    }
    
    /// <summary>
    /// Gets the bit at the given index from the left.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="index">The bit index.</param>
    /// <returns>Returns if the bit is set to 1.</returns>
    public static bool GetBitFromLeft(ulong value, int index)
    {
        return (value & (1ul << (63 - index))) != 0;
    }
    
    public static byte GetBitsFromLeft(byte value, byte start, byte length)
    {
        return (byte)((byte)(value << start) >> (8 - length));
    }
    
    public static ushort GetBitsFromLeft(ushort value, byte start, byte length)
    {
        return (ushort)((ushort)(value << start) >> (16 - length));
    }
    
    public static uint GetBitsFromLeft(uint value, byte start, byte length)
    {
        return (value << start) >> (32 - length);
    }
    
    public static ulong GetBitsFromLeft(ulong value, byte start, byte length)
    {
        return (value << start) >> (64 - length);
    }
}