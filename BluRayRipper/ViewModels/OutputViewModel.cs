using System.ComponentModel;
using Avalonia.Controls;
using BluRayRipper.Models.Output;
using BluRayRipper.Views;

namespace BluRayRipper.ViewModels;

public class OutputViewModel : ViewModelBase
{
    /// <summary>
    /// Gets the title output instance.
    /// </summary>
    public OutputModel Output { get; }
    
    public OutputViewModel(OutputModel output)
    {
        Output = output;
        Output.PropertyChanged += OutputOnPropertyChanged;
    }

    /// <summary>
    /// Gets the display name.
    /// </summary>
    public string Name => Output.BaseName;
    
    

    private void OutputOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(OutputModel.Progress):
                OnPropertyChanged(nameof(Progress));
                break;
            case nameof(OutputModel.Status):
                OnPropertyChanged(nameof(IsProgressBarVisible));
                break;
        }
        
    }

    /// <summary>
    /// Gets the current export progress of this file.
    /// </summary>
    public double Progress => Output.Progress;
    
    /// <summary>
    /// Gets if the progress bar is visible.
    /// </summary>
    public bool IsProgressBarVisible => Output.Status == OutputStatus.Running;
    
    /// <inheritdoc />
    public override Control CreateView()
    {
        return new OutputView();
    }
}