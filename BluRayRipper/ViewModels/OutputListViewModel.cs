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
    private readonly IOutputQueueService _outputQueueService;

    public OutputListViewModel(IOutputService outputService, IOutputQueueService outputQueueService)
    {
        _outputService = outputService;
        _outputQueueService = outputQueueService;

        _outputService.Items.MapAndObserve(Items, ModelToViewModel);
        _outputQueueService.QueueProgressChanged += OutputQueueServiceOnQueueProgressChanged;
    }

    private void OutputQueueServiceOnQueueProgressChanged(object? sender, EventArgs e)
    {
    }


    /// <summary>
    /// Gets the list of outputs.
    /// </summary>
    public ObservableCollection<OutputFileViewModel> Items { get; } = [];

    /// <summary>
    /// Refreshes the output list.
    /// </summary>
    public async Task RefreshAsync()
    {
        await _outputService.RefreshAsync();
    }

    private OutputFileViewModel ModelToViewModel(OutputFile output)
    {
        var viewModel = new OutputFileViewModel(output);
        if (_outputQueueService.TryGetProcess(output, out var queueProgress))
            viewModel.QueueProgress = queueProgress;
        return viewModel;
    }
    
    /// <inheritdoc />
    public override Control CreateView()
    {
        return new OutputListView();
    }
}