namespace BluRayLib.FFmpeg;

public class StreamMetadata
{
    /// <summary>
    /// Gets the stream id.
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
    /// Gets the stream type.
    /// </summary>
    public StreamType Type { get; set; }

    /// <summary>
    /// Gets the language of the stream.
    /// </summary>
    public string Language { get; set; } = "";

    /// <summary>
    /// Gets the format string of the stream.
    /// </summary>
    public string Format { get; set; } = "";

    public override string ToString()
    {
        return $"Stream #{InputId}:{Id}({Language}): {Type}: {Format}: {Title ?? "-/-"}";
    }
}