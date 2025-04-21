namespace BluRayLib.Ripper.Output;

/// <summary>
/// Contains the information about a title export. This allows the application to remember a previous export.
/// </summary>
[Serializable]
public class OutputInfo
{
    /// <summary>
    /// Gets and sets the source this output was generated from.
    /// </summary>
    public OutputSource Source { get; set; } = new();

    /// <summary>
    /// Gets and sets the name of this file.
    /// </summary>
    public string Name { get; set; } = "";
    
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