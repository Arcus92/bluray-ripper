using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using MediaRipper.Services.Interfaces;
using MediaRipper.Views;

namespace MediaRipper.ViewModels;

public class OutputSelectorViewModel : ViewModelBase
{
    private readonly ISettingService _settingService;
    private readonly IOutputService _outputService;
    private readonly IStorageProviderAccessor _storageProviderAccessor;
    
    public OutputSelectorViewModel(ISettingService settingService, IOutputService outputService, IStorageProviderAccessor storageProviderAccessor)
    {
        _settingService = settingService;
        _outputService = outputService;
        _storageProviderAccessor = storageProviderAccessor;

        _outputPath = _settingService.OutputPath;
        
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
    
    /// <summary>
    /// Opens and loads the current output path.
    /// </summary>
    public async Task OpenAsync()
    {
        await _outputService.OpenAsync(_outputPath);
        _settingService.OutputPath = _outputPath;
    }
    
    /// <summary>
    /// Opens a folder picker to select the output file.
    /// </summary>
    public async Task OpenFolderPickerAsync()
    {
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
        await _outputService.OpenAsync(_outputPath);
    }
    
    /// <inheritdoc />
    public override Control CreateView()
    {
        return new OutputSelectorView();
    }
}