namespace MediaRipper.Models.MediaLookup;

/// <summary>
/// The season info of a <see cref="MediaType.Tv"/> show.
/// </summary>
public class MediaSeason
{
    /// <summary>
    /// Gets the internal id of the season.
    /// </summary>
    public required int Id { get; init; }

    /// <summary>
    /// Gets the series id.
    /// </summary>
    public required int SeriesId { get; init; }

    /// <summary>
    /// Gets the name of the season.
    /// </summary>
    public required string Name { get; init; }
    
    /// <summary>
    /// Gets the seasons's description.
    /// </summary>
    public required string Description { get; init; }
    
    /// <summary>
    /// Gets the season number.
    /// </summary>
    public required int SeasonNumber { get; init; }
    
    /// <summary>
    /// Gets the number of episodes in this season.
    /// </summary>
    public int EpisodeCount { get; init; }
}