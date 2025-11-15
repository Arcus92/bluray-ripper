namespace MediaLib.TheMovieDatabase.Models;

/// <summary>
/// The full movie details.
/// </summary>
[Serializable]
public class MovieDetails
{
    /// <summary>
    /// Gets the movie id.
    /// </summary>
    public int Id { get; init; }
    
    /// <summary>
    /// Gets the movie title.
    /// </summary>
    public string Title { get; init; } = "";
    
    /// <summary>
    /// Gets the movie's original language.
    /// </summary>
    public string OriginalLanguage { get; init; } = "";
    
    /// <summary>
    /// Gets the movie's original title.
    /// </summary>
    public string OriginalTitle { get; init; } = "";
    
    /// <summary>
    /// Gets the description of the movie.
    /// </summary>
    public string Overview { get; init; } = "";
    
    /// <summary>
    /// Gets the release date.
    /// </summary>
    public DateTime ReleaseDate { get; init; }
}