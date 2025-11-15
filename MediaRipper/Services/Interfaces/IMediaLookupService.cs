using System.Threading.Tasks;
using MediaLib.TheMovieDatabase;
using MediaRipper.Models.MediaLookup;

namespace MediaRipper.Services.Interfaces;

public interface IMediaLookupService
{
    /// <summary>
    /// Gets TheMovieDatabase api service.
    /// </summary>
    TheMovieDatabaseApi MovieDatabaseApi { get; }
    
    /// <summary>
    /// Gets the default language.
    /// </summary>
    string Language { get; }
    
    /// <summary>
    /// Searches the given query and returns the found media results.
    /// </summary>
    /// <param name="query">The search term.</param>
    /// <returns>Returns the list of search results.</returns>
    Task<MediaSearchResult[]> SearchAsync(string query);
    
    /// <summary>
    /// Fetches the details of the given media item.
    /// </summary>
    /// <param name="mediaType">The media type to fetch.</param>
    /// <param name="id">The id of the media item.</param>
    /// <returns>Returns the media details.</returns>
    Task<MediaDetails> GetDetailsAsync(MediaType mediaType, int id);
    
    /// <summary>
    /// Fetches the details of the given media item.
    /// </summary>
    /// <param name="searchResult">The search result to fetch the details from.</param>
    /// <returns>Returns the media details.</returns>
    Task<MediaDetails> GetDetailsAsync(MediaSearchResult searchResult) => GetDetailsAsync(searchResult.MediaType, searchResult.Id);

    /// <summary>
    /// Fetches the season details of the given series.
    /// </summary>
    /// <param name="seriesId">The series id.</param>
    /// <param name="seasonNumber">The season number.</param>
    /// <returns>Returns the season details.</returns>
    Task<MediaSeasonDetails> GetSeasonDetailsAsync(int seriesId, int seasonNumber);
    
    /// <summary>
    /// Fetches the season details of the given series.
    /// </summary>
    /// <param name="season">The season to fetch.</param>
    /// <returns>Returns the season details.</returns>
    Task<MediaSeasonDetails> GetSeasonDetailsAsync(MediaSeason season) => GetSeasonDetailsAsync(season.SeriesId, season.SeasonNumber);
}