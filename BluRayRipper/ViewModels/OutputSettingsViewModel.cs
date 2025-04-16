using System.Threading.Tasks;
using Avalonia.Controls;
using BluRayRipper.Models.Output;
using BluRayRipper.Services.Interfaces;
using BluRayRipper.Views;

namespace BluRayRipper.ViewModels;

public class OutputSettingsViewModel : ViewModelBase
{
    private readonly IDiskService _diskService;
    private readonly IOutputService _outputService;
    private readonly IOutputQueueService _outputQueueService;

    public OutputSettingsViewModel(IDiskService diskService, IOutputService outputService, IOutputQueueService outputQueueService)
    {
        _diskService = diskService;
        _outputService = outputService;
        _outputQueueService = outputQueueService;
    }
    
    /// <inheritdoc cref="IOutputService.RefreshAsync"/>
    public async Task RefreshAsync()
    {
        await _outputService.RefreshAsync();
    }

    public void StartQueue()
    {
        _outputQueueService.Start();
    }
    
    /// <inheritdoc />
    public override Control CreateView()
    {
        return new OutputSettingsView();
    }
}