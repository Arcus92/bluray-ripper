namespace BluRayLib.Ripper;

public readonly struct DiskInfo
{
    /// <summary>
    /// Gets the disk name.
    /// </summary>
    public string DiskName { get; init; }
    
    /// <summary>
    /// Gets the content hash.
    /// </summary>
    public string ContentHash { get; init; }
}