namespace MediaLib.Output;

/// <summary>
/// Defined a single file from one <see cref="OutputDefinition"/>. One file can contain multiple streams.
/// </summary>
[Serializable]
public class OutputFile
{
    /// <summary>
    /// Gets and sets the filename.
    /// </summary>
    public string Filename { get; set; } = "";
    
    /// <summary>
    /// Gets and sets the FFmpeg output format.
    /// </summary>
    public string Format { get; set; } = "";
    
    /// <summary>
    /// Gets and sets the list of streams in this file.
    /// </summary>
    public OutputStream[] Streams { get; set; } = [];
}