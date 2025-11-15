namespace MediaRipper.Models.MediaLookup;

public class MediaEpisode
{
    /// <summary>
    /// Gets the internal id of the episode.
    /// </summary>
    public required int Id { get; init; }
    
    /// <summary>
    /// Gets the name of the episode.
    /// </summary>
    public required string Name { get; init; }
    
    /// <summary>
    /// Gets the episode's description.
    /// </summary>
    public required string Description { get; init; }
    
    /// <summary>
    /// Gets the season number.
    /// </summary>
    public required int SeasonNumber { get; init; }
    
    /// <summary>
    /// Gets the episode number.
    /// </summary>
    public required int EpisodeNumber { get; init; }
}