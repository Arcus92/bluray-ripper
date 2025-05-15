using System;
using MediaLib.Models;

namespace MediaRipper.Models.Nodes;

public class ChapterNode : BaseNode
{
    public ChapterNode(ChapterInfo chapter)
    {
        Chapter = chapter;
    }

    /// <summary>
    /// Gets the chapter info.
    /// </summary>
    public ChapterInfo Chapter { get; }
    
    /// <summary>
    /// Gets the chapter name.
    /// </summary>
    public string Name => Chapter.Name;
    
    /// <summary>
    /// Gets the chapter start.
    /// </summary>
    public TimeSpan Start => Chapter.Start;
    
    /// <summary>
    /// Gets the chapter send.
    /// </summary>
    public TimeSpan End => Chapter.End;
    
    /// <summary>
    /// Gets the display name of this chapter.
    /// </summary>
    public string DisplayName => Chapter.ToString();
}