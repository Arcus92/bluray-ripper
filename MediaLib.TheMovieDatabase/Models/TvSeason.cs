namespace MediaLib.TheMovieDatabase.Models;

/// <summary>
/// The TV season info.
/// </summary>
[Serializable]
public class TvSeason
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
    /// Gets the episode count.
    /// </summary>
    public int EpisodeCount { get; init; }
    
    /// <summary>
    /// Gets the season number.
    /// </summary>
    public int SeasonNumber { get; init; }
    
    /// <summary>
    /// Gets the date of the season's airing.
    /// </summary>
    public DateTime AirDate { get; init; }
}