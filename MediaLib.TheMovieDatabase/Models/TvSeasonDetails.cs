namespace MediaLib.TheMovieDatabase.Models;

/// <summary>
/// The full TV season details.
/// </summary>
[Serializable]
public class TvSeasonDetails
{
    /// <summary>
    /// Gets the season id.
    /// </summary>
    public int Id { get; init; }
    
    /// <summary>
    /// Gets the name of the season.
    /// </summary>
    public string Name { get; init; } = "";
    
    /// <summary>
    /// Gets the description of the season.
    /// </summary>
    public string Overview { get; init; } = "";
    
    /// <summary>
    /// Gets the season number.
    /// </summary>
    public int SeasonNumber { get; init; }
    
    /// <summary>
    /// Gets the airing date.
    /// </summary>
    public DateTime AirDate { get; init; }

    /// <summary>
    /// Gets the episodes.
    /// </summary>
    public TvEpisode[] Episodes { get; init; } = [];
}