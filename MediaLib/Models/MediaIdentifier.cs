using System.Text.Json.Serialization;
using MediaLib.Sources;

namespace MediaLib.Models;

/// <summary>
/// Identifier of a <see cref="IMediaSource"/>.
/// </summary>
[Serializable]
public class MediaIdentifier : IEquatable<MediaIdentifier>
{
    /// <summary>
    /// Gets and sets the source type.
    /// </summary>
    public MediaIdentifierType Type { get; init; }
    
    /// <summary>
    /// Gets and sets the disk name from which this output file was generated.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string DiskName { get; init; } = "";
    
    /// <summary>
    /// Gets and sets the content hash of the source.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string ContentHash { get; init; } = "";
    
    /// <summary>
    /// Gets and sets the track id from which this output file was generated.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ushort Id { get; init; }
    
    #region Equals

    public bool Equals(MediaIdentifier? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Type == other.Type && ContentHash == other.ContentHash && Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((MediaIdentifier)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine((int)Type, ContentHash, Id);
    }

    #endregion Equals
}