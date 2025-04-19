using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
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
    public OutputModel Model { get; }
    
    public OutputViewModel(IOutputService outputService, OutputModel model)
    {
        _outputService = outputService;
        
        Model = model;
        Model.PropertyChanged += ModelOnPropertyChanged;
        
        // Build externals
        Files = new ObservableCollection<OutputFileViewModel>(model.Files.Select(f => new OutputFileViewModel(f)));
    }

    /// <inheritdoc cref="OutputModel.Name"/>
    public string Name => Model.Name;
    
    /// <inheritdoc cref="OutputModel.Progress"/>
    public double Progress => Model.Progress;
    
    /// <inheritdoc cref="OutputModel.Status"/>
    public OutputStatus Status => Model.Status;
    
    /// <summary>
    /// Gets if the progress bar is visible.
    /// </summary>
    public bool IsProgressBarVisible => Model.Status == OutputStatus.Running;

    private void ModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
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

    /// <summary>
    /// Gets a collection of all external streams.
    /// </summary>
    public ObservableCollection<OutputFileViewModel> Files { get; }

    /// <summary>
    /// Renames all files.
    /// </summary>
    public async Task RenameAsync()
    {
        // TODO
    }
    
    #endregion Rename
    
    /// <inheritdoc />
    public override Control CreateView()
    {
        return new OutputView();
    }
}