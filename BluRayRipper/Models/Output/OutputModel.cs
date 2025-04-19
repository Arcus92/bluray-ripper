using System.Linq;
using BluRayLib.Ripper.Output;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BluRayRipper.Models.Output;

public class OutputModel : ObservableObject
{
    /// <summary>
    /// Gets the output file data.
    /// </summary>
    public OutputInfo Info { get; }

    public OutputModel(OutputInfo info)
    {
        Info = info;
        Files = info.Files.Select(f => new OutputFileModel(f)).ToArray();
    }

    /// <summary>
    /// Gets the file models.
    /// </summary>
    public OutputFileModel[] Files { get; }

    /// <inheritdoc cref="OutputInfo.Name"/>
    public string Name => Info.Name;
    
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