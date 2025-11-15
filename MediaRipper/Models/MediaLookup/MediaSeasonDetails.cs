namespace MediaRipper.Models.MediaLookup;

/// <summary>
/// The season details of a <see cref="MediaType.Tv"/> show.
/// </summary>
public class MediaSeasonDetails
{
    /// <summary>
    /// Gets the internal id of the season.
    /// </summary>
    public required int Id { get; init; }
    
    /// <summary>
    /// Gets the name of the season.
    /// </summary>
    public required string Name { get; init; }
    
    /// <summary>
    /// Gets the season number.
    /// </summary>
    public required int SeasonNumber { get; init; }
    
    /// <summary>
    /// Gets the episodes of the season.
    /// </summary>
    public MediaEpisode[] Episodes { get; init; } = [];
}