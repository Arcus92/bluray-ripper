namespace MediaLib.Models;

public class ChapterInfo
{
    public ChapterInfo()
    {
    }

    /// <summary>
    /// Gets the chapter index.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// Gets the chapter name.
    /// </summary>
    public string Name { get; set; } = "";
    
    /// <summary>
    /// Gets the start time of the chapter.
    /// </summary>
    public TimeSpan Start { get; set; }
    
    /// <summary>
    /// Gets the end time of the chapter.
    /// </summary>
    public TimeSpan End { get; set; }
    
    /// <summary>
    /// Gets the start time of the chapter.
    /// </summary>
    public TimeSpan Duration => End - Start;
    
    public override string ToString() => $"Chapter #{Index} - \"{Name}\" [{Start:hh\\:mm\\:ss} - {End:hh\\:mm\\:ss}]";
}