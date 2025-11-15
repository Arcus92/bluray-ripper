using System;

namespace MediaRipper.Models.MediaLookup;

/// <summary>
/// The model of the media lookup search result.
/// </summary>
public class MediaSearchResult
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
}