namespace MediaLib.Models;

public class SegmentInfo : IMediaInfo
{
    /// <summary>
    /// Gets the clip id of the segment.
    /// </summary>
    public required ushort Id { get; init; }

    /// <summary>
    /// Gets the segment description.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the duration.
    /// </summary>
    public TimeSpan Duration { get; init; }
    
    /// <summary>
    /// Gets the video streams.
    /// </summary>
    public VideoInfo[] VideoStreams { get; init; } = [];
    
    /// <summary>
    /// Gets the audio streams.
    /// </summary>
    public AudioInfo[] AudioStreams { get; init; } = [];
    
    /// <summary>
    /// Gets the subtitle streams.
    /// </summary>
    public SubtitleInfo[] SubtitleStreams { get; init; } = [];
    
    /// <inheritdoc />
    public override string ToString() => Name;
}