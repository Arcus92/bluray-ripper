using MediaLib.TheMovieDatabase.Models;
using MediaLib.TheMovieDatabase.Utils;

namespace MediaLib.TheMovieDatabase.Services;

public readonly struct MovieService(TheMovieDatabaseApi api)
{
    /// <summary>
    /// Fetches the movie details.
    /// </summary>
    /// <param name="seriesId">The id of the movie.</param>
    /// <param name="language">The result language.</param>
    /// <returns>Returns the movie details.</returns>
    public async Task<MovieDetails> DetailsAsync(int seriesId, string? language = null)
    {
        var builder = new UriQueryBuilder($"{api.Endpoint}/3/movie/{seriesId}");
        
        if (language is not null) builder.Add("language", language);
        
        return await api.GetAsync<MovieDetails>(builder.ToString());
    }
}