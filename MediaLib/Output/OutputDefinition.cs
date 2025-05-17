using MediaLib.Models;

namespace MediaLib.Output;

/// <summary>
/// Contains the information about a title export. This allows the application to remember a previous export.
/// </summary>
[Serializable]
public class OutputDefinition
{
    /// <summary>
    /// Gets and sets the media identifier this output was generated from.
    /// </summary>
    public required MediaIdentifier Identifier { get; init; }
    
    /// <summary>
    /// Gets and sets the media info.
    /// </summary>
    public OutputMediaInfo MediaInfo { get; set; } = new();

    /// <summary>
    /// Gets and sets the duration.
    /// </summary>
    public TimeSpan Duration { get; set; }
    
    /// <summary>
    /// Gets and sets the codec options.
    /// </summary>
    public CodecOptions Codec { get; set; } = new();
    
    /// <summary>
    /// Gets and sets the files of this export.
    /// </summary>
    public OutputFile[] Files { get; set; } = [];

    /// <summary>
    /// Gets and sets if chapters should be exported.
    /// </summary>
    public bool ExportChapters { get; set; } = true;
    
    /// <summary>
    /// Gets and sets the chapters.
    /// </summary>
    public OutputChapter[] Chapters { get; set; } = [];
}