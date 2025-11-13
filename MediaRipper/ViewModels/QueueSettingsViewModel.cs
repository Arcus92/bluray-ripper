using System.Threading.Tasks;
using Avalonia.Controls;
using MediaRipper.Services.Interfaces;
using MediaRipper.Views;

namespace MediaRipper.ViewModels;

public class QueueSettingsViewModel(IOutputService outputService, IOutputQueueService outputQueueService)
    : ViewModelBase
{
    /// <inheritdoc cref="IOutputService.RefreshAsync"/>
    public async Task RefreshAsync()
    {
        await outputService.RefreshAsync();
    }

    /// <inheritdoc cref="IOutputQueueService.Start"/>
    public void StartQueue()
    {
        outputQueueService.Start();
    }
    
    /// <inheritdoc cref="IOutputQueueService.Stop"/>
    public void StopQueue()
    {
        outputQueueService.Stop();
    }
    
    /// <inheritdoc />
    public override Control CreateView()
    {
        return new QueueSettingsView();
    }
}