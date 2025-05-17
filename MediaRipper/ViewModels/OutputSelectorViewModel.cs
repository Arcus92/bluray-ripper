using System;
using Avalonia.Controls;
using MediaRipper.Services.Interfaces;
using MediaRipper.Views;

namespace MediaRipper.ViewModels;

public class OutputSelectorViewModel : ViewModelBase
{
    private readonly ISettingService _settingService;
    private readonly IOutputService _outputService;
    private readonly IMediaProviderService _mediaProviderService;
    
    public OutputSelectorViewModel(ISettingService settingService, IOutputService outputService, IMediaProviderService mediaProviderService)
    {
        _settingService = settingService;
        _outputService = outputService;
        _mediaProviderService = mediaProviderService;

        _outputPath = _settingService.GetDefaultOutputPath();
        _mediaProviderService.Changed += OnMediaProviderChanged;

        _outputService.OpenAsync(_outputPath);
    }
    private void OnMediaProviderChanged(object? sender, EventArgs e)
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