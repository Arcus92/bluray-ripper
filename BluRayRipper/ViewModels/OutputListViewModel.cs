using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Avalonia.Controls;
using BluRayRipper.Models.Output;
using BluRayRipper.Services.Interfaces;
using BluRayRipper.Utils;
using BluRayRipper.Views;

namespace BluRayRipper.ViewModels;

public class OutputListViewModel : ViewModelBase
{
    private readonly IOutputService _outputService;

    public OutputListViewModel(IOutputService outputService)
    {
        _outputService = outputService;

        _outputService.Items.MapAndObserve(Items, ModelToViewModel);
    }
    
    /// <summary>
    /// Gets the list of outputs.
    /// </summary>
    public ObservableCollection<OutputViewModel> Items { get; } = [];

    private OutputViewModel ModelToViewModel(OutputModel output)
    {
        var viewModel = new OutputViewModel(output);
        return viewModel;
    }
    
    /// <inheritdoc />
    public override Control CreateView()
    {
        return new OutputListView();
    }
}