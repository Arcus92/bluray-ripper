using System;

namespace MediaRipper.Models.MediaLookup;

public class MediaDetails
{
    /// <summary>
    /// Gets the internal id of the media item.
    /// </summary>
    public required int Id { get; init; }
    
    /// <summary>
    /// Gets the name of the media item.
    /// </summary>
    public required string Name { get; init; }
    
    /// <summary>
    /// Gets the media description.
    /// </summary>
    public required string Description { get; init; }
    
    /// <summary>
    /// Gets the media type.
    /// </summary>
    public required MediaType MediaType { get; init; }
    
    /// <summary>
    /// Gets the release date.
    /// </summary>
    public DateTime ReleaseDate { get; init; }

    /// <summary>
    /// Gets the number of seasons.
    /// </summary>
    public int SeasonCount { get; init; }
    
    /// <summary>
    /// Gets the number of episodes.
    /// </summary>
    public int EpisodeCount { get; init; }
    
    /// <summary>
    /// Gets the seasons for a TV show.
    /// </summary>
    public MediaSeason[] Seasons { get; init; } = [];
}