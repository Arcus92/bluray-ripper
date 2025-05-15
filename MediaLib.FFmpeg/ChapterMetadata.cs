namespace MediaLib.FFmpeg;

public class ChapterMetadata
{
    /// <summary>
    /// Gets the chapter id.
    /// </summary>
    public ulong Id { get; set; }

    /// <summary>
    /// Gets the input id.
    /// </summary>
    public ulong InputId { get; set; }
    
    /// <summary>
    /// Gets the title.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets the start of the chapter.
    /// </summary>
    public TimeSpan Start { get; set; }
    
    /// <summary>
    /// Gets the end of the chapter.
    /// </summary>
    public TimeSpan End { get; set; }
    
    public override string ToString()
    {
        return $"Chapter #{InputId}:{Id}: {Title ?? "-/-"}";
    }
}