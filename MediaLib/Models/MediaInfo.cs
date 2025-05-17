namespace MediaLib.Models;

public class MediaInfo : IMediaInfo
{
    /// <summary>
    /// Gets the media id.
    /// </summary>
    public required ushort Id { get; init; }
    
    /// <summary>
    /// Gets the description if this media item.
    /// </summary>
    public required string Name { get; init; }
    
    /// <summary>
    /// Gets the playlist duration.
    /// </summary>
    public TimeSpan Duration { get; set; }
    
    /// <summary>
    /// Gets the segments / clips.
    /// </summary>
    public SegmentInfo[] Segments { get; set; } = [];

    /// <summary>
    /// Gets the chapters.
    /// </summary>
    public ChapterInfo[] Chapters { get; set; } = [];
    
    /// <inheritdoc />
    public override string ToString() => Name;
}