using System.Collections.ObjectModel;
using System.Linq;
using BluRayLib.Ripper.Info;

namespace BluRayRipper.Models.Nodes;

public class TitleNode : BaseNode
{
    public TitleNode(PlaylistInfo playlist)
    {
        Playlist = playlist;

        var segmentNode = new TextNode("Segments")
        {
            IsExpanded = true,
            SubNodes = new ObservableCollection<BaseNode>(Playlist.Segments.Select(s => new SegmentNode(s) { IsChecked = true }))
        };
        var chapterNode = new TextNode("Chapters")
        {
            IsExpanded = true,
            SubNodes = new ObservableCollection<BaseNode>(Playlist.Chapters.Select(c => new ChapterNode(c) { IsChecked = true }))
        };
        SubNodes = [ segmentNode, chapterNode ];
    }

    /// <summary>
    /// Gets the playlist info.
    /// </summary>
    public PlaylistInfo Playlist { get; }
    
    /// <summary>
    /// Gets the title id.
    /// </summary>
    public ushort Id => Playlist.Id;
    
    /// <summary>
    /// Gets the title display name.
    /// </summary>
    public string DisplayName => Playlist.ToString();
    
    /// <summary>
    /// Gets the sub-nodes.
    /// </summary>
    public ObservableCollection<BaseNode> SubNodes { get; }
    
    /// <inheritdoc cref="IsChecked"/>
    private bool _isChecked;
    
    /// <summary>
    /// Gets and sets if this title is selected for export.
    /// </summary>
    public bool IsChecked
    {
        get => _isChecked;
        set => SetProperty(ref _isChecked, value);
    }
}