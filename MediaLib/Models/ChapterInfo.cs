namespace MediaLib.Models;

public class ChapterInfo : IMediaInfo
{
    /// <summary>
    /// Gets the chapter index.
    /// </summary>
    public required ushort Id { get; init; }

    /// <summary>
    /// Gets the chapter description.
    /// </summary>
    public required string Name { get; init; }
    
    /// <summary>
    /// Gets the start time of the chapter.
    /// </summary>
    public TimeSpan Start { get; init; }
    
    /// <summary>
    /// Gets the end time of the chapter.
    /// </summary>
    public TimeSpan End { get; init; }
    
    /// <summary>
    /// Gets the start time of the chapter.
    /// </summary>
    public TimeSpan Duration => End - Start;
    
    /// <inheritdoc />
    public override string ToString() => Name;
}