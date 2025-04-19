namespace BluRayLib.Ripper.Output;

/// <summary>
/// Defines a chapter in the <see cref="OutputFile"/>.
/// </summary>
[Serializable]
public class OutputChapter
{
    /// <summary>
    /// Gets and sets the chapter name.
    /// </summary>
    public string Name { get; set; } = "";
    
    /// <summary>
    /// Gets and sets the start time of the chapter.
    /// </summary>
    public TimeSpan Start { get; set; }
    
    /// <summary>
    /// Gets and sets the end time of the chapter.
    /// </summary>
    public TimeSpan End { get; set; }
}