namespace MediaLib.TheMovieDatabase.Models;

/// <summary>
/// The search result for a movie.
/// </summary>
[Serializable]
public class MovieResult : SearchResult
{
    /// <summary>
    /// Gets the movie title.
    /// </summary>
    public string Title { get; init; } = "";
    
    /// <summary>
    /// Gets the movie title in the original language.
    /// </summary>
    public string OriginalTitle { get; init; } = "";
    
    /// <summary>
    /// Gets the movie description.
    /// </summary>
    public string Overview  { get; init; } = "";
    
    /// <summary>
    /// Gets the genre ids.
    /// </summary>
    public int[] GenreIds { get; init; } = [];
    
    /// <summary>
    /// Gets the release date.
    /// </summary>
    public DateTime ReleaseDate { get; init; }
    
    /// <summary>
    /// Gets if this is adult content.
    /// </summary>
    public bool Adult { get; init; }
}