using System;
using BluRayLib.Ripper.Info;

namespace BluRayRipper.Models.Nodes;

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
    
    /// <inheritdoc cref="IsChecked"/>
    private bool _isChecked;
    
    /// <summary>
    /// Gets and sets if this chapter marker is selected for export.
    /// </summary>
    public bool IsChecked
    {
        get => _isChecked;
        set => SetProperty(ref _isChecked, value);
    }
}