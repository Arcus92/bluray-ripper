using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia.Controls;
using BluRayLib.Ripper.BluRays.Export;
using BluRayRipper.Models.Output;
using BluRayRipper.Services.Interfaces;
using BluRayRipper.Views;

namespace BluRayRipper.ViewModels;

public class OutputViewModel : ViewModelBase
{
    private readonly IOutputService _outputService;
    
    /// <summary>
    /// Gets the title output instance.
    /// </summary>
    public OutputModel Output { get; }
    
    public OutputViewModel(IOutputService outputService, OutputModel output)
    {
        _outputService = outputService;
        
        Output = output;
        Output.PropertyChanged += OutputOnPropertyChanged;
        BaseName = Output.BaseName;
    }

    /// <inheritdoc cref="OutputModel.BaseName"/>
    public string Name => Output.BaseName;
    
    /// <inheritdoc cref="OutputModel.Progress"/>
    public double Progress => Output.Progress;
    
    /// <inheritdoc cref="OutputModel.Status"/>
    public OutputStatus Status => Output.Status;
    
    /// <summary>
    /// Gets if the progress bar is visible.
    /// </summary>
    public bool IsProgressBarVisible => Output.Status == OutputStatus.Running;

    private void OutputOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(OutputModel.Progress):
                OnPropertyChanged(nameof(Progress));
                break;
            case nameof(OutputModel.Status):
                OnPropertyChanged(nameof(Status));
                OnPropertyChanged(nameof(IsProgressBarVisible));
                break;
        }
    }

    #region Rename

    private string _baseName;

    public string BaseName
    {
        get => _baseName;
        set => SetProperty(ref _baseName, value);
    }
    
    /// <summary>
    /// Applies the <see cref="BaseName"/> 
    /// </summary>
    public async Task RenameAsync()
    {
        if (BaseName == Output.BaseName) return;
        if (Status != OutputStatus.Completed) return;

        var nameMap = new TitleNameMap();
        nameMap.Add(0, $"{BaseName}{Output.Extension}");
        
        await _outputService.RenameAsync(Output.File, nameMap);
    }
    
    #endregion Rename
    
    /// <inheritdoc />
    public override Control CreateView()
    {
        return new OutputView();
    }
}