namespace MediaLib.Models;

public class SegmentInfo
{
    public SegmentInfo(ushort id)
    {
        Id = id;
    }

    /// <summary>
    /// Gets the clip id of the segment.
    /// </summary>
    public ushort Id { get; }
    
    /// <summary>
    /// Gets the duration.
    /// </summary>
    public TimeSpan Duration { get; set; }
    
    /// <summary>
    /// Gets the video streams.
    /// </summary>
    public VideoInfo[] VideoStreams { get; set; } = [];
    
    /// <summary>
    /// Gets the audio streams.
    /// </summary>
    public AudioInfo[] AudioStreams { get; set; } = [];
    
    /// <summary>
    /// Gets the subtitle streams.
    /// </summary>
    public SubtitleInfo[] SubtitleStreams { get; set; } = [];
    
    public override string ToString() => $"Segment [0x{Id:x4}] ({Id:00000}) - [{Duration:hh\\:mm\\:ss}]";
}