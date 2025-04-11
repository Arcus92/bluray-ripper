namespace BluRayRipper.Services.Interfaces;

public interface ISettingService
{
    /// <summary>
    /// Returns the default disk path.
    /// </summary>
    /// <returns>The last used path to the disk.</returns>
    string GetDefaultDiskPath();
    
    /// <summary>
    /// Returns the default output path.
    /// </summary>
    /// <returns>The last used path to the output.</returns>
    string GetDefaultOutputPath();
}