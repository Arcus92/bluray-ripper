using MediaLib.Utils.IO;

namespace MediaLib.Utils;

/// <summary>
/// Encoder for the RunLengthEncoding.
/// </summary>
public static class RunLengthEncoding
{
    /// <summary>
    /// Encodes the data using the RunLengthEncoding.
    /// </summary>
    /// <param name="data">The data to encode.</param>
    /// <param name="setter">The callback to set a pixel.</param>
    public static void Encode(ReadOnlySpan<byte> data, Action<ushort, ushort, byte> setter)
    {
        var reader = new ByteReader(data);
        Encode(reader, setter, (c, _, x, y, b) =>
        {
            c(x, y, b);
        });
    }
    
    /// <summary>
    /// Encodes the data using the RunLengthEncoding.
    /// </summary>
    /// <param name="data">The data to encode.</param>
    /// <param name="setter">The callback to set a pixel.</param>
    public static void Encode(ReadOnlySpan<byte> data, Action<int, byte> setter)
    {
        var reader = new ByteReader(data);
        Encode(reader, setter, (c, p, _, _, b) =>
        {
            c(p, b);
        });
    }
    
    /// <summary>
    /// Encodes the data using the RunLengthEncoding.
    /// </summary>
    /// <param name="data">The data to encode.</param>
    /// <param name="width">The width of the image.</param>
    /// <param name="height">The height of the image.</param>
    /// <returns>Returns the byte array.</returns>
    public static byte[] Encode(ReadOnlySpan<byte> data, ushort width, ushort height)
    {
        var reader = new ByteReader(data);
        var array = new byte[width * height];
        Encode(reader, array, (a, p, _, _, b) =>
        {
            a[p] = b;
        });
        return array;
    }

    /// <summary>
    /// Encodes the data using the RunLengthEncoding.
    /// </summary>
    /// <param name="data">The data to encode.</param>
    /// <param name="setterObject">An object that is passed to the <paramref name="setter"/> to avoid closure allocations.</param>
    /// <param name="setter">The callback to set a pixel.</param>
    /// <typeparam name="T">The custom <paramref name="setterObject"/> type.</typeparam>
    public static void Encode<T>(ReadOnlySpan<byte> data, T setterObject, Action<T, ushort, ushort, byte> setter)
    {
        var reader = new ByteReader(data);
        var param = (setter, setterObject);
        Encode(reader, param, (o, _, x, y, b) =>
        {
            o.setter(o.setterObject, x, y, b);
        });
    }
    
    /// <summary>
    /// Encodes the data using the RunLengthEncoding.
    /// </summary>
    /// <param name="data">The data to encode.</param>
    /// <param name="setterObject">An object that is passed to the <paramref name="setter"/> to avoid closure allocations.</param>
    /// <param name="setter">The callback to set a pixel.</param>
    /// <typeparam name="T">The custom <paramref name="setterObject"/> type.</typeparam>
    public static void Encode<T>(ReadOnlySpan<byte> data, T setterObject, Action<T, int, byte> setter)
    {
        var reader = new ByteReader(data);
        var param = (setter, setterObject);
        Encode(reader, param, (o, p, _, _, b) =>
        {
            o.setter(o.setterObject, p, b);
        });
    }

    /// <summary>
    /// Encodes the data using the RunLengthEncoding.
    /// </summary>
    /// <param name="reader">The data reader to encode.</param>
    /// <param name="setterObject">An object that is passed to the <paramref name="setter"/> to avoid closure allocations.</param>
    /// <param name="setter">The callback to set a pixel.</param>
    /// <typeparam name="T">The custom <paramref name="setterObject"/> type.</typeparam>
    public static void Encode<T>(ByteReader reader, T setterObject, Action<T, int, ushort, ushort, byte> setter)
    {
        ushort x = 0;
        ushort y = 0;
        var p = 0;
        while (reader.Position < reader.Length)
        {
            var byte1 = reader.ReadByte();
            if (byte1 != 0x00)
            {
                setter(setterObject, p++, x++, y, byte1);
                continue;
            }

            var byte2 = reader.ReadByte();
            if (byte2 == 0x00) // End of line
            {
                x = 0;
                y++;
                continue;
            }
            var bit8 = (byte2 & 0b10000000) != 0;
            var bit7 = (byte2 & 0b01000000) != 0;
            var num = byte2 & 0b00111111;
            if (bit7)
            {
                num = (num << 8) + reader.ReadByte();
            }
            var index = bit8 ? reader.ReadByte() : (byte)0x00;
                
            for (var i = 0; i < num; i++)
            {
                setter(setterObject, p++, x++, y, index);
            }
        }
    }
}