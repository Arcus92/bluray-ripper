using System.Threading.Tasks;
using Avalonia.Controls;
using MediaRipper.Services.Interfaces;
using MediaRipper.Views;

namespace MediaRipper.ViewModels;

public class DiskSelectorViewModel : ViewModelBase
{
    private readonly ISettingService _settingService;
    private readonly IMediaProviderService _mediaProviderService;

    public DiskSelectorViewModel(ISettingService settingService, IMediaProviderService mediaProviderService)
    {
        _settingService = settingService;
        _mediaProviderService = mediaProviderService;
        
        _diskPath = _settingService.GetDefaultDiskPath();
    }
    
    /// <inheritdoc cref="DiskPath"/>
    private string _diskPath;

    /// <summary>
    /// Gets and sets the disk input path.
    /// </summary>
    public string DiskPath
    {
        get => _diskPath;
        set => SetProperty(ref _diskPath, value);
    }
    
    public async Task OpenAsync()
    {
        await _mediaProviderService.OpenAsync(_diskPath);
    }
    
    /// <inheritdoc />
    public override Control CreateView()
    {
        return new DiskSelectorView();
    }
}