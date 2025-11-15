namespace MediaLib.TheMovieDatabase.Models;

/// <summary>
/// The TV episode info.
/// </summary>
[Serializable]
public class TvEpisode
{
    /// <summary>
    /// Gets the episode id.
    /// </summary>
    public int Id { get; init; }
    
    /// <summary>
    /// Gets the airing date.
    /// </summary>
    public DateTime AirDate { get; init; }
    
    /// <summary>
    /// Gets the id of the TV show.
    /// </summary>
    public int ShowId { get; init; }
    
    /// <summary>
    /// Gets the season number.
    /// </summary>
    public int SeasonNumber { get; init; }
    
    /// <summary>
    /// Gets the episode number.
    /// </summary>
    public int EpisodeNumber { get; init; }

    /// <summary>
    /// Gets the episode name.
    /// </summary>
    public string Name { get; init; } = "";
    
    /// <summary>
    /// Gets the episode overview.
    /// </summary>
    public string Overview { get; init; } = "";

    /// <summary>
    /// Gets the episode runtime in minutes.
    /// </summary>
    public int Runtime { get; init; }
}