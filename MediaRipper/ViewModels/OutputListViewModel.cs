using System.Collections.ObjectModel;
using Avalonia.Controls;
using MediaRipper.Models.Output;
using MediaRipper.Services.Interfaces;
using MediaRipper.Utils;
using MediaRipper.Views;

namespace MediaRipper.ViewModels;

public class OutputListViewModel : ViewModelBase
{
    private readonly IOutputService _outputService;

    public OutputListViewModel(IOutputService outputService)
    {
        _outputService = outputService;

        _outputService.Outputs.MapAndObserve(Items, ModelToViewModel);
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
        return new OutputListView();
    }
}