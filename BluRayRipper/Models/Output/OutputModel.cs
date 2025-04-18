using CommunityToolkit.Mvvm.ComponentModel;

namespace BluRayRipper.Models.Output;

public class OutputModel : ObservableObject
{
    /// <summary>
    /// Gets the output file data.
    /// </summary>
    public OutputFile File { get; }

    public OutputModel(OutputFile file)
    {
        File = file;
    }
    
    /// <inheritdoc cref="OutputFile.PlaylistId"/>
    public ushort PlaylistId => File.PlaylistId;
    
    /// <inheritdoc cref="OutputFile.DiskName"/>
    public string DiskName => File.DiskName;
    
    /// <inheritdoc cref="OutputFile.BaseName"/>
    public string BaseName => File.BaseName;
    
    /// <inheritdoc cref="Status"/>
    private OutputStatus _status;

    /// <summary>
    /// Gets and sets the status.
    /// </summary>
    public OutputStatus Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }
    
    /// <inheritdoc cref="Progress"/>
    private double _progress;

    /// <summary>
    /// Gets and sets the progress of the current export.
    /// </summary>
    public double Progress
    {
        get => _progress;
        set => SetProperty(ref _progress, value);
    }
    
    
}