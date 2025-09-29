using System.Collections.ObjectModel;
using Avalonia.Controls;
using MediaRipper.Models.Outputs;
using MediaRipper.Services.Interfaces;
using MediaRipper.Utils;
using MediaRipper.Views;

namespace MediaRipper.ViewModels;

public class OutputTreeViewModel : ViewModelBase
{
    private readonly IOutputService _outputService;

    public OutputTreeViewModel(IOutputService outputService)
    {
        _outputService = outputService;

        _outputService.Outputs.MapAndObserve(Items, ModelToViewModel);
    }
    
    /// <inheritdoc cref="SelectedItem"/>
    private OutputViewModel? _selectedItem;

    /// <summary>
    /// Gets and sets the selected title info.
    /// </summary>
    public OutputViewModel? SelectedItem
    {
        get => _selectedItem;
        set => SetProperty(ref _selectedItem, value);
    }
    
    /// <summary>
    /// Gets the list of outputs.
    /// </summary>
    public ObservableCollection<OutputViewModel> Items { get; } = [];

    private OutputViewModel ModelToViewModel(OutputModel output)
    {
        var viewModel = new OutputViewModel(_outputService, output);
        return viewModel;
    }
    
    /// <inheritdoc />
    public override Control CreateView()
    {
        return new OutputTreeView();
    }
}