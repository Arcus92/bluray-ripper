using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using MediaRipper.Models.Outputs;
using MediaRipper.Services.Interfaces;
using MediaRipper.Views;

namespace MediaRipper.ViewModels;

public class OutputSelectorViewModel : ViewModelBase
{
    private readonly ISettingService _settingService;
    private readonly IOutputService _outputService;
    private readonly IStorageProviderAccessor _storageProviderAccessor;
    private readonly IOutputQueueService _outputQueueService;
    
    public OutputSelectorViewModel(ISettingService settingService, IOutputService outputService, 
        IStorageProviderAccessor storageProviderAccessor, IOutputQueueService outputQueueService)
    {
        _settingService = settingService;
        _outputService = outputService;
        _storageProviderAccessor = storageProviderAccessor;
        _outputQueueService = outputQueueService;

        _outputPath = _settingService.OutputPath;
        _outputQueueService.StatusChanged += OnOutputQueueServiceStatusChanged;
        
        _ = OpenAsync();
    }
    
    /// <inheritdoc cref="OutputPath"/>
    private string _outputPath;

    /// <summary>
    /// Gets and sets the output path.
    /// </summary>
    public string OutputPath
    {
        get => _outputPath;
        set => SetProperty(ref _outputPath, value);
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
    /// Opens and loads the current output path.
    /// </summary>
    public async Task OpenAsync()
    {
        if (!IsEnabled) return;
        await _outputService.OpenAsync(_outputPath);
        _settingService.OutputPath = _outputPath;
    }
    
    /// <summary>
    /// Opens a folder picker to select the output file.
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
            SuggestedStartLocation = await storageProvider.TryGetFolderFromPathAsync(_outputPath)
        });

        if (paths.Count >= 1)
        {
            OutputPath = paths[0].Path.AbsolutePath;
            await OpenAsync();
        }
    }
    
    /// <summary>
    /// Refresh the current output path.
    /// </summary>
    public async Task RefreshAsync()
    {
        if (!IsEnabled) return;
        await _outputService.OpenAsync(_outputPath);
    }
    
    /// <inheritdoc />
    public override Control CreateView()
    {
        return new OutputSelectorView();
    }
}