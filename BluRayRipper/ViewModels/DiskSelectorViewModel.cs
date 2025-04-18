using System.Threading.Tasks;
using Avalonia.Controls;
using BluRayRipper.Services.Interfaces;
using BluRayRipper.Views;

namespace BluRayRipper.ViewModels;

public class DiskSelectorViewModel : ViewModelBase
{
    private readonly ISettingService _settingService;
    private readonly IDiskService _diskService;

    public DiskSelectorViewModel(ISettingService settingService, IDiskService diskService)
    {
        _settingService = settingService;
        _diskService = diskService;
        
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
        await _diskService.OpenAsync(_diskPath);
    }
    
    /// <inheritdoc />
    public override Control CreateView()
    {
        return new DiskSelectorView();
    }
}