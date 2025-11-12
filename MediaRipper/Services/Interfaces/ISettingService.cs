namespace MediaRipper.Services.Interfaces;

public interface ISettingService
{
    /// <summary>
    /// Loads the settings file.
    /// </summary>
    void Load();
    
    /// <summary>
    /// Saves the settings file.
    /// </summary>
    void Save();
    
    /// <summary>
    /// Gets and sets the last opened source path.
    /// </summary>
    string SourcePath { get; set; }
    
    /// <summary>
    /// Gets and sets the last opened output path.
    /// </summary>
    string OutputPath { get; set; }
}