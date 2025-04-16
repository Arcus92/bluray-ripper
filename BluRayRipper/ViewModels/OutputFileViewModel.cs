using System.ComponentModel;
using Avalonia.Controls;
using BluRayRipper.Models.Output;
using BluRayRipper.Views;

namespace BluRayRipper.ViewModels;

public class OutputFileViewModel : ViewModelBase
{
    /// <summary>
    /// Gets the title output instance.
    /// </summary>
    public OutputFile OutputFile { get; }
    
    public OutputFileViewModel(OutputFile outputFile)
    {
        OutputFile = outputFile;
    }

    /// <summary>
    /// Gets the display name.
    /// </summary>
    public string Name => OutputFile.BaseName;
    
    
    #region Progress

    /// <inheritdoc cref="QueueProgress"/>
    private OutputProgress? _outputProgress;

    /// <summary>
    /// Gets and sets the queue progress object if this output item is currently queued.
    /// </summary>
    public OutputProgress? QueueProgress
    {
        get => _outputProgress;
        set
        {
            if (_outputProgress == value) return;
            if (_outputProgress is not null) _outputProgress.PropertyChanged -= OutputProgressOnPropertyChanged;
            SetProperty(ref _outputProgress, value);
            if (_outputProgress is not null) _outputProgress.PropertyChanged += OutputProgressOnPropertyChanged;
        }
    }

    private void OutputProgressOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(OutputProgress.Progress):
                OnPropertyChanged(nameof(Progress));
                break;
        }
        
    }

    /// <summary>
    /// Gets the current export progress of this file.
    /// </summary>
    public double Progress => QueueProgress?.Progress ?? 0.0;
    
    /// <summary>
    /// Gets if the progress bar is visible.
    /// </summary>
    public bool IsProgressBarVisible => QueueProgress is not null;
    
    #endregion Progress
    
    /// <inheritdoc />
    public override Control CreateView()
    {
        return new OutputFileView();
    }
}