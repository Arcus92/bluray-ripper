using MediaLib.TheMovieDatabase.Models;
using MediaLib.TheMovieDatabase.Utils;

namespace MediaLib.TheMovieDatabase.Services;

public readonly struct TvService(TheMovieDatabaseApi api)
{
    /// <summary>
    /// Fetches the series info.
    /// </summary>
    /// <param name="seriesId">The id of the TV show.</param>
    /// <param name="language">The result language.</param>
    /// <returns>Returns the series information.</returns>
    public async Task<TvSeriesDetails> SeriesAsync(int seriesId, string? language = null)
    {
        var builder = new UriQueryBuilder($"{api.Endpoint}/3/tv/{seriesId}");
        
        if (language is not null) builder.Add("language", language);
        
        return await api.GetAsync<TvSeriesDetails>(builder.ToString());
    }
    
    /// <summary>
    /// Fetches the season info of the given show.
    /// </summary>
    /// <param name="seriesId">The id of the TV show.</param>
    /// <param name="seasonNumber">The season number.</param>
    /// <param name="language">The result language.</param>
    /// <returns>Returns the season information.</returns>
    public async Task<TvSeasonDetails> SeasonAsync(int seriesId, int seasonNumber, string? language = null)
    {
        var builder = new UriQueryBuilder($"{api.Endpoint}/3/tv/{seriesId}/season/{seasonNumber}");
        
        if (language is not null) builder.Add("language", language);
        
        return await api.GetAsync<TvSeasonDetails>(builder.ToString());
    }
}