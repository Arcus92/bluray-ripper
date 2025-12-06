using Avalonia.Controls;
using MediaRipper.Services.Interfaces;
using MediaRipper.Views;

namespace MediaRipper.ViewModels;

public class SettingsWindowViewModel : ViewModelBase
{
    private readonly ISettingService _settingService;
    private readonly IApplicationService _applicationService;
    
    public SettingsWindowViewModel(IApplicationService applicationService, ISettingService settingService)
    {
        _settingService = settingService;
        _applicationService = applicationService;

        WriteSettingsToProperties();
    }

    #region Properties

    /// <summary>
    /// Gets and sets the FFmpeg path.
    /// </summary>
    public string? FFmpegPath
    {
        get;
        set => SetProperty(ref field, value);
    }

    /// <summary>
    /// Gets and sets the FFplay path.
    /// </summary>
    public string? FFplayPath
    {
        get;
        set => SetProperty(ref field, value);
    }
    
    /// <summary>
    /// Gets and sets the TheMovieDatabase api-key.
    /// </summary>
    public string? TheMovieDatabaseApiKey
    {
        get;
        set => SetProperty(ref field, value);
    }
    
    /// <summary>
    /// Gets and sets the TheMovieDatabase api-key.
    /// </summary>
    public string? TheMovieDatabaseLanguage
    {
        get;
        set => SetProperty(ref field, value);
    }

    #endregion Properties

    /// <summary>
    /// Saves the settings and closes the window.
    /// </summary>
    public void Apply()
    {
        WritePropertiesToSettings();
        
        _applicationService.CloseWindow(this);
    }
    
    private void WriteSettingsToProperties()
    {
        FFmpegPath = _settingService.Data.FFmpegPath;
        FFplayPath = _settingService.Data.FFplayPath;
        TheMovieDatabaseApiKey = _settingService.Data.TheMovieDatabase.ApiKey;
        TheMovieDatabaseLanguage = _settingService.Data.TheMovieDatabase.Language;
    }

    private void WritePropertiesToSettings()
    {
        _settingService.Data.FFmpegPath = FFmpegPath;
        _settingService.Data.FFplayPath = FFplayPath;
        _settingService.Data.TheMovieDatabase.ApiKey = TheMovieDatabaseApiKey;
        _settingService.Data.TheMovieDatabase.Language = TheMovieDatabaseLanguage;
        _settingService.NotifyChange();
    }
    
    /// <inheritdoc />
    public override Control CreateView()
    {
        return new SettingsWindow();
    }
}