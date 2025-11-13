using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using MediaRipper.Models.Outputs;
using MediaRipper.Services.Interfaces;
using MediaRipper.Views;

namespace MediaRipper.ViewModels;

public class SourceSelectorViewModel : ViewModelBase
{
    private readonly ISettingService _settingService;
    private readonly IMediaProviderService _mediaProviderService;
    private readonly IStorageProviderAccessor _storageProviderAccessor;
    private readonly IOutputQueueService _outputQueueService;

    public SourceSelectorViewModel(ISettingService settingService, IMediaProviderService mediaProviderService, 
        IStorageProviderAccessor storageProviderAccessor, IOutputQueueService outputQueueService)
    {
        _settingService = settingService;
        _mediaProviderService = mediaProviderService;
        _storageProviderAccessor = storageProviderAccessor;
        _outputQueueService = outputQueueService;
        
        _sourcePath = _settingService.SourcePath;
        _outputQueueService.StatusChanged += OnOutputQueueServiceStatusChanged;
    }

    /// <inheritdoc cref="SourcePath"/>
    private string _sourcePath;

    /// <summary>
    /// Gets and sets the disk input path.
    /// </summary>
    public string SourcePath
    {
        get => _sourcePath;
        set => SetProperty(ref _sourcePath, value);
    }
    
    /// <inheritdoc cref="IsEnabled"/>
    private bool _isEnabled = true;

    /// <summary>
    /// Gets if this element is enabled.
    /// </summary>
    public bool IsEnabled
    {
        get => _isEnabled;
        private set => SetProperty(ref _isEnabled, value);
    }
    
    private void OnOutputQueueServiceStatusChanged(object? sender, EventArgs e)
    {
        UpdateStatus();
    }

    private void UpdateStatus()
    {
        IsEnabled = _outputQueueService.Status != OutputQueueStatus.Running;
    }
    
    /// <summary>
    /// Opens and loads the current source path.
    /// </summary>
    public async Task OpenAsync()
    {
        if (!IsEnabled) return;
        await _mediaProviderService.OpenAsync(_sourcePath);
        _settingService.SourcePath = _sourcePath;
    }

    /// <summary>
    /// Opens a folder picker to select the source file.
    /// </summary>
    public async Task OpenFolderPickerAsync()
    {
        if (!IsEnabled) return;
        var storageProvider = _storageProviderAccessor.StorageProvider;
        if (storageProvider is null)
        {
            return;
        }
        
        var paths = await storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            AllowMultiple = false,
            SuggestedStartLocation = await storageProvider.TryGetFolderFromPathAsync(_sourcePath)
        });

        if (paths.Count >= 1)
        {
            SourcePath = paths[0].Path.AbsolutePath;
            await OpenAsync();
        }
    }
    
    /// <inheritdoc />
    public override Control CreateView()
    {
        return new SourceSelectorView();
    }
}