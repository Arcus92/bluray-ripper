namespace DvdLib;

public class Dvd
{
    /// <summary>
    /// Gets the Dvd disk path.
    /// </summary>
    public string DiskPath { get; }

    /// <summary>
    /// Gets the Dvd disk name.
    /// </summary>
    public string DiskName { get; }
    
    public Dvd(string diskPath)
    {
        diskPath = Path.GetFullPath(diskPath).TrimEnd('/', '\\'); // Sanitize
        DiskPath = diskPath;
        DiskName = Path.GetFileName(diskPath);
    }
    
    #region Info
    
    /// <summary>
    /// Gets the content hash of the disc. This content hash is compatible with TheDiscDb.
    /// </summary>
    public string ContentHash { get; private set; } = "";
    
    #endregion Info
}