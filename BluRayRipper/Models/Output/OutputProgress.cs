using CommunityToolkit.Mvvm.ComponentModel;

namespace BluRayRipper.Models.Output;

public class OutputProgress : ObservableObject
{
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