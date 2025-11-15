namespace MediaLib.TheMovieDatabase.Models;

/// <summary>
/// The search result for a TV series.
/// </summary>
[Serializable]
public class TvResult : SearchResult
{
    /// <summary>
    /// Gets the name of the series.
    /// </summary>
    public string Name { get; init; } = "";
    
    /// <summary>
    /// Gets the original language of the series.
    /// </summary>
    public string OriginalLanguage { get; init; } = "";
    
    /// <summary>
    /// Gets the name of the series in the original language.
    /// </summary>
    public string OriginalName { get; init; } = "";
    
    /// <summary>
    /// Gets the series description.
    /// </summary>
    public string Overview  { get; init; } = "";
    
    /// <summary>
    /// Gets the genre ids.
    /// </summary>
    public int[] GenreIds { get; init; } = [];
    
    /// <summary>
    /// Gets if this is adult content.
    /// </summary>
    public bool Adult { get; init; }
    
    /// <summary>
    /// Gets the data of the first airing.
    /// </summary>
    public DateTime FirstAirDate { get; init; }
    
    /// <summary>
    /// Gets the url of the backdrop image.
    /// </summary>
    public string? BackdropPath { get; init; }
}