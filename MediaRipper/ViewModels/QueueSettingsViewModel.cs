using System;
using Avalonia.Controls;
using MediaRipper.Models.Outputs;
using MediaRipper.Services.Interfaces;
using MediaRipper.Views;

namespace MediaRipper.ViewModels;

public class QueueSettingsViewModel : ViewModelBase
{
    private readonly IApplicationService _applicationService;
    private readonly IOutputQueueService _outputQueueService;
    private readonly IMediaProviderService _mediaProviderService;

    public QueueSettingsViewModel(IApplicationService applicationService, IOutputQueueService outputQueueService, 
        IMediaProviderService mediaProviderService)
    {
        _applicationService = applicationService;
        _outputQueueService = outputQueueService;
        _mediaProviderService = mediaProviderService;
        
        _outputQueueService.StatusChanged += OnOutputQueueServiceStatusChanged;
        _mediaProviderService.Changed += OnMediaProviderServiceChanged;
    }

    #region Queue
    
    /// <inheritdoc cref="IsRunning" />
    private bool _isRunning;

    /// <summary>
    /// Gets if the queue is started.
    /// </summary>
    public bool IsRunning
    {
        get => _isRunning;
        set => SetProperty(ref _isRunning, value);
    }
    
    /// <inheritdoc cref="CanStartQueue" />
    private bool _canStartQueue;

    /// <summary>
    /// Gets if the queue can be started.
    /// </summary>
    public bool CanStartQueue
    {
        get => _canStartQueue;
        set => SetProperty(ref _canStartQueue, value);
    }
    
    private void OnOutputQueueServiceStatusChanged(object? sender, EventArgs e)
    {
        UpdateQueue();
    }

    private void OnMediaProviderServiceChanged(object? sender, EventArgs e)
    {
        UpdateQueue();
    }

    private void UpdateQueue()
    {
        IsRunning = _outputQueueService.Status == OutputQueueStatus.Running;
        CanStartQueue = _mediaProviderService.IsLoaded;
    }
    
    #endregion Queue
    
    #region Commands
    
    /// <inheritdoc cref="IOutputQueueService.Start"/>
    public void StartQueue()
    {
        _outputQueueService.Start();
    }
    
    /// <inheritdoc cref="IOutputQueueService.Stop"/>
    public void StopQueue()
    {
        _outputQueueService.Stop();
    }

    /// <summary>
    /// Opens the application settings.
    /// </summary>
    public void OpenSettings()
    {
        _applicationService.ShowWindow<SettingsWindowViewModel>();
    }
    
    #endregion Commands
    
    /// <inheritdoc />
    public override Control CreateView()
    {
        return new QueueSettingsView();
    }
}