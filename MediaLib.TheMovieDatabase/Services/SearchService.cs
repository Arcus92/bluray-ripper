using MediaLib.TheMovieDatabase.Models;
using MediaLib.TheMovieDatabase.Utils;

namespace MediaLib.TheMovieDatabase.Services;

/// <summary>
/// TheMovieDatabase search api.
/// </summary>
/// <param name="api">The main api reference.</param>
public readonly struct SearchService(TheMovieDatabaseApi api)
{
    /// <summary>
    /// Searches TV shows by the given query.
    /// </summary>
    /// <param name="query">The query text.</param>
    /// <param name="firstAirDateYear">Search only the first air date.</param>
    /// <param name="includeAdult">Should adult content be included?</param>
    /// <param name="language">The result language.</param>
    /// <param name="page">The search page to return.</param>
    /// <param name="year">Search the first air date and all episode air dates.</param>
    /// <returns>Returns the result list.</returns>
    public async Task<SearchResults<TvResult>> TvAsync(string query, int? firstAirDateYear = null, bool? includeAdult = null, string? language = null, int? page = null, int? year = null)
    {
        var builder = new UriQueryBuilder($"{api.Endpoint}/3/search/tv");
        
        builder.Add("query", query);
        if (firstAirDateYear.HasValue) builder.Add("first_air_date_year", firstAirDateYear.Value);
        if (includeAdult.HasValue) builder.Add("include_adult", includeAdult.Value);
        if (language is not null) builder.Add("language", language);
        if (page.HasValue) builder.Add("page", page.Value);
        if (year.HasValue) builder.Add("year", year.Value);
        
        return await api.GetAsync<SearchResults<TvResult>>(builder.ToString());
    }
    
    /// <summary>
    /// Searches movies by the given query.
    /// </summary>
    /// <param name="query">The query text.</param>
    /// <param name="includeAdult">Should adult content be included?</param>
    /// <param name="language">The result language.</param>
    /// <param name="page">The search page to return.</param>
    /// <returns>Returns the result list.</returns>
    public async Task<SearchResults<MovieResult>> MovieAsync(string query, bool? includeAdult = null, string? language = null, int? page = null)
    {
        var builder = new UriQueryBuilder($"{api.Endpoint}/3/search/movie");
        
        builder.Add("query", query);
        if (includeAdult.HasValue) builder.Add("include_adult", includeAdult.Value);
        if (language is not null) builder.Add("language", language);
        if (page.HasValue) builder.Add("page", page.Value);
        
        return await api.GetAsync<SearchResults<MovieResult>>(builder.ToString());
    }
    
    /// <summary>
    /// Searches all media types by the given query.
    /// </summary>
    /// <param name="query">The query text.</param>
    /// <param name="includeAdult">Should adult content be included?</param>
    /// <param name="language">The result language.</param>
    /// <param name="page">The search page to return.</param>
    /// <returns>Returns the result list.</returns>
    public async Task<SearchResults<SearchResult>> MultiAsync(string query, bool? includeAdult = null, string? language = null, int? page = null)
    {
        var builder = new UriQueryBuilder($"{api.Endpoint}/3/search/multi");
        
        builder.Add("query", query);
        if (includeAdult.HasValue) builder.Add("include_adult", includeAdult.Value);
        if (language is not null) builder.Add("language", language);
        if (page.HasValue) builder.Add("page", page.Value);
        
        return await api.GetAsync<SearchResults<SearchResult>>(builder.ToString());
    }
}