namespace MediaLib.FFmpeg;

public class InputMetadata
{
    /// <summary>
    /// Gets the input id.
    /// </summary>
    public ulong Id { get; set; }

    /// <summary>
    /// Gets the title.
    /// </summary>
    public string? Title { get; set; }
    
    /// <summary>
    /// Gets the used encoder.
    /// </summary>
    public string? Encoder { get; set; }

    /// <summary>
    /// Gets the duration.
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Gets the chapters.
    /// </summary>
    public ChapterMetadata[] Chapters { get; set; } = Array.Empty<ChapterMetadata>();
    
    /// <summary>
    /// Gets the stream infos.
    /// </summary>
    public StreamMetadata[] Streams { get; set; } = Array.Empty<StreamMetadata>();
}