namespace BluRayLib.Ripper.BluRays;

public class SegmentData
{
    public SegmentData(ushort id)
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
    public VideoData[] VideoStreams { get; set; } = [];
    
    /// <summary>
    /// Gets the audio streams.
    /// </summary>
    public AudioInfo[] AudioStreams { get; set; } = [];
    
    /// <summary>
    /// Gets the subtitle streams.
    /// </summary>
    public SubtitleData[] SubtitleStreams { get; set; } = [];
    
    public override string ToString() => $"Segment [0x{Id:x4}] ({Id:00000}) - [{Duration:hh\\:mm\\:ss}]";
}