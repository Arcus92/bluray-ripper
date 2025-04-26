using System;
using Avalonia.Controls;
using BluRayRipper.Services.Interfaces;
using BluRayRipper.Views;

namespace BluRayRipper.ViewModels;

public class OutputSelectorViewModel : ViewModelBase
{
    private readonly ISettingService _settingService;
    private readonly IOutputService _outputService;
    private readonly IDiskService _diskService;
    
    public OutputSelectorViewModel(ISettingService settingService, IOutputService outputService, IDiskService diskService)
    {
        _settingService = settingService;
        _outputService = outputService;
        _diskService = diskService;

        _outputPath = _settingService.GetDefaultOutputPath();
        _diskService.Loaded += OnDiskLoaded;

        _outputService.OpenAsync(_outputPath);
    }
    private void OnDiskLoaded(object? sender, EventArgs e)
    {
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
    
    /// <inheritdoc />
    public override Control CreateView()
    {
        return new OutputSelectorView();
    }
}