using System.Text.RegularExpressions;

namespace DvdLib.Vmg;

public readonly partial struct VmgIdentifier : IEquatable<VmgIdentifier>
{
    /// <summary>
    /// Gets the titleset (01..99).
    /// </summary>
    public byte TitleSet { get; init; }
    
    /// <summary>
    /// Gets the sub-index in the title set (0..9).
    /// </summary>
    public byte Index { get; init; }

    public VmgIdentifier(byte titleSet, byte index)
    {
        TitleSet = titleSet;
        Index = index;
    }
    
    /// <summary>
    /// Returns the segment id.
    /// </summary>
    /// <returns>Returns the segment id.</returns>
    public ushort ToSegmentId()
    {
        return (ushort)(TitleSet << 8 | Index);
    }
    
    /// <summary>
    /// Returns the filename of this identifier.
    /// </summary>
    /// <returns>Returns the filename as string.</returns>
    public string ToFilename()
    {
        if (TitleSet == 0 && Index == 0)
        {
            return "VIDEO_TS";
        }
        
        return $"VTS_{TitleSet:00}_{Index}";
    }
    

    /// <summary>
    /// Returns the identifier of the first index (0) in the current title set.
    /// </summary>
    /// <returns></returns>
    public VmgIdentifier Root => new(TitleSet, 0);
    
    /// <inheritdoc />
    public override string ToString() => ToFilename();

    #region Equals

    /// <inheritdoc />
    public bool Equals(VmgIdentifier other)
    {
        return TitleSet == other.TitleSet && Index == other.Index;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is VmgIdentifier other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(TitleSet, Index);
    }

    public static bool operator ==(VmgIdentifier left, VmgIdentifier right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(VmgIdentifier left, VmgIdentifier right)
    {
        return !left.Equals(right);
    }

    #endregion Equals
    
    #region Static
    
    [GeneratedRegex(@"VTS_(\d\d)_(\d)")]
    private static partial Regex FilenameRegex();
    
    /// <summary>
    /// Converts the given segment id to an identifier.
    /// </summary>
    /// <param name="segmentId">The segment id.</param>
    /// <returns>Returns the identifier.</returns>
    public static VmgIdentifier FromSegmentId(ushort segmentId)
    {
        var titleSet = (byte)(segmentId >> 8);
        var index = (byte)segmentId;
        return new VmgIdentifier(titleSet, index);
    }

    /// <summary>
    /// Converts the given filename into an identifier.
    /// </summary>
    /// <param name="filename">The filename without file extension.</param>
    /// <returns>Returns the identifier.</returns>
    public static VmgIdentifier FromFilename(string filename)
    {
        if (filename == "VIDEO_TS")
        {
            return new VmgIdentifier();
        }
        
        var match = FilenameRegex().Match(filename);
        if (!match.Success)
            throw new ArgumentException($"Unknown filename: {filename}!");
        
        var titleSet = byte.Parse(match.Groups[1].Value);
        var index = byte.Parse(match.Groups[2].Value);
        return new VmgIdentifier(titleSet, index);
    }
    
    #endregion Static
}