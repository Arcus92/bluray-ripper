using System.Threading.Tasks;
using Avalonia.Controls;
using MediaRipper.Services.Interfaces;
using MediaRipper.Views;

namespace MediaRipper.ViewModels;

public class QueueSettingsViewModel : ViewModelBase
{
    private readonly IMediaProviderService _mediaProviderService;
    private readonly IOutputService _outputService;
    private readonly IOutputQueueService _outputQueueService;

    public QueueSettingsViewModel(IMediaProviderService mediaProviderService, IOutputService outputService, IOutputQueueService outputQueueService)
    {
        _mediaProviderService = mediaProviderService;
        _outputService = outputService;
        _outputQueueService = outputQueueService;
    }
    
    /// <inheritdoc cref="IOutputService.RefreshAsync"/>
    public async Task RefreshAsync()
    {
        await _outputService.RefreshAsync();
    }

    /// <inheritdoc cref="IOutputQueueService.StartAsync"/>
    public async Task StartQueueAsync()
    {
        await _outputQueueService.StartAsync();
    }
    
    /// <inheritdoc />
    public override Control CreateView()
    {
        return new QueueSettingsView();
    }
}