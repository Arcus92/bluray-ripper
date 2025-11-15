using System.Net.Http;
using MediaLib.TheMovieDatabase;
using MediaRipper.Services.Interfaces;

namespace MediaRipper.Services;

/// <summary>
/// The service to lookup media information from an online service.
/// </summary>
public class MediaLookupService : IMediaLookupService
{
    private readonly ISettingService _settingService;
    
    public MediaLookupService(IHttpClientFactory httpClientFactory, ISettingService settingService)
    {
        _settingService = settingService;

        MovieDatabaseApi = new TheMovieDatabaseApi(httpClientFactory, settingService.Data.TheMovieDatabase.ApiKey);
    }

    /// <inheritdoc />
    public TheMovieDatabaseApi MovieDatabaseApi { get; }

    /// <inheritdoc />
    public string Language => _settingService.Data.TheMovieDatabase.Language;
}