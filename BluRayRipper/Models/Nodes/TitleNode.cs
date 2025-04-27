using System.Collections.ObjectModel;
using System.Linq;
using BluRayLib.Ripper.BluRays;

namespace BluRayRipper.Models.Nodes;

public class TitleNode : BaseNode
{
    public TitleNode(TitleData playlist)
    {
        Playlist = playlist;

        SegmentNode = new TextNode<SegmentNode>("Segments")
        {
            IsExpanded = true,
            SubNodes = new ObservableCollection<BaseNode>(Playlist.Segments.Select(s => new SegmentNode(s)))
        };
        ChapterNode = new TextNode<ChapterNode>("Chapters")
        {
            IsExpanded = true,
            SubNodes = new ObservableCollection<BaseNode>(Playlist.Chapters.Select(c => new ChapterNode(c)))
        };
        SubNodes = [ SegmentNode, ChapterNode ];
    }

    /// <summary>
    /// Gets the playlist info.
    /// </summary>
    public TitleData Playlist { get; }
    
    /// <summary>
    /// Gets the title id.
    /// </summary>
    public ushort Id => Playlist.Id;
    
    /// <summary>
    /// Gets the title display name.
    /// </summary>
    public string DisplayName => Playlist.ToString();
    
    /// <summary>
    /// Gets the segment category node.
    /// </summary>
    public TextNode<SegmentNode> SegmentNode { get; }
    
    /// <summary>
    /// Gets the chapter category node.
    /// </summary>
    public TextNode<ChapterNode> ChapterNode { get; }
    
    /// <summary>
    /// Gets the sub-nodes.
    /// </summary>
    public ObservableCollection<TextNode> SubNodes { get; }
    
    
    /// <inheritdoc cref="IsIgnored"/>
    private bool _isIgnored;
    
    /// <summary>
    /// Gets and sets if this title is ignored by default.
    /// </summary>
    public bool IsIgnored
    {
        get => _isIgnored;
        set => SetProperty(ref _isIgnored, value);
    }
}