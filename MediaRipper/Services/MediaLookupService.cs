using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MediaLib.TheMovieDatabase;
using MediaLib.TheMovieDatabase.Models;
using MediaRipper.Models.MediaLookup;
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

    /// <inheritdoc />
    public async Task<MediaSearchResult[]> SearchAsync(string query)
    {
        var response = await MovieDatabaseApi.Search.MultiAsync(query, language: Language);
        var list = new List<MediaSearchResult>();

        foreach (var result in response.Results)
        {
            switch (result)
            {
                case TvResult tvResult:
                    list.Add(new MediaSearchResult
                    {
                        Id = tvResult.Id,
                        MediaType = MediaType.Tv,
                        Name = tvResult.Name,
                        Description = tvResult.Overview,
                        ReleaseDate = tvResult.FirstAirDate,
                    });
                    break;
                case MovieResult movieResult:
                    list.Add(new MediaSearchResult
                    {
                        Id = movieResult.Id,
                        MediaType = MediaType.Movie,
                        Name = movieResult.Title,
                        Description = movieResult.Overview,
                        ReleaseDate = movieResult.ReleaseDate
                    });
                    break;
            }
        }
        
        return list.ToArray();
    }

    /// <inheritdoc />
    public async Task<MediaDetails> GetDetailsAsync(MediaType mediaType, int id)
    {
        return mediaType switch
        {
            MediaType.Movie => await GetMovieDetailsAsync(id),
            MediaType.Tv => await GetTvSeriesDetailsAsync(id),
            _ => throw new ArgumentException($"Invalid media type {mediaType}", nameof(mediaType))
        };
    }

    private async Task<MediaDetails> GetMovieDetailsAsync(int id)
    {
        var response = await MovieDatabaseApi.Movie.DetailsAsync(id, language: Language);
        return new MediaDetails
        {
            Id = response.Id,
            MediaType = MediaType.Movie,
            Name = response.Title,
            Description = response.Overview,
            ReleaseDate = response.ReleaseDate,
        };
    }
    
    private async Task<MediaDetails> GetTvSeriesDetailsAsync(int id)
    {
        var response = await MovieDatabaseApi.Tv.DetailsAsync(id, language: Language);
        return new MediaDetails
        {
            Id = response.Id,
            MediaType = MediaType.Tv,
            Name = response.Name,
            Description = response.Overview,
            ReleaseDate = response.FirstAirDate,
            SeasonCount = response.NumberOfSeasons,
            EpisodeCount = response.NumberOfEpisodes,
            Seasons = response.Seasons.Select(s => new MediaSeason
            {
                Id = s.Id,
                Name = s.Name,
                SeriesId = id,
                SeasonNumber = s.SeasonNumber,
                EpisodeCount = s.EpisodeCount,
            }).ToArray()
        };
    }
    
    /// <inheritdoc />
    public async Task<MediaSeasonDetails> GetSeasonDetailsAsync(int seriesId, int seasonNumber)
    {
        var response = await MovieDatabaseApi.Tv.SeasonAsync(seriesId, seasonNumber, language: Language);
        return new MediaSeasonDetails
        {
            Id = response.Id,
            Name = response.Name,
            SeasonNumber = response.SeasonNumber,
            Episodes = response.Episodes.Select(e => new MediaEpisode
            {
                Id = e.Id,
                Name = e.Name,
                SeasonNumber = e.SeasonNumber,
                EpisodeNumber = e.EpisodeNumber,
                Description = e.Overview
            }).ToArray()
        };
    }
}