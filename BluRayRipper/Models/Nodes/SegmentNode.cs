using System.Collections.ObjectModel;
using System.Linq;
using BluRayLib.Ripper.BluRays;

namespace BluRayRipper.Models.Nodes;

public class SegmentNode : BaseNode
{
    public SegmentNode(SegmentData segment)
    {
        Segment = segment;
        
        var videoStreamNode = new TextNode("Videos")
        {
            IsExpanded = true,
            SubNodes = new ObservableCollection<BaseNode>(Segment.VideoStreams.Select(s => new VideoNode(s) { IsChecked = true }))
        };
        var audioStreamNode = new TextNode("Audios")
        {
            IsExpanded = true,
            SubNodes = new ObservableCollection<BaseNode>(Segment.AudioStreams.Select(s => new AudioNode(s) { IsChecked = true }))
        };
        var subtitleStreamNode = new TextNode("Subtitles")
        {
            IsExpanded = true,
            SubNodes = new ObservableCollection<BaseNode>(Segment.SubtitleStreams.Select(s => new SubtitleNode(s) { IsChecked = true }))
        };
        SubNodes = [ videoStreamNode, audioStreamNode, subtitleStreamNode ];
    }
    
    /// <summary>
    /// Gets the segment info.
    /// </summary>
    public SegmentData Segment { get; }
    
    /// <summary>
    /// Gets the segment id.
    /// </summary>
    public ushort Id => Segment.Id;

    /// <summary>
    /// Gets the display name.
    /// </summary>
    public string DisplayName => Segment.ToString();
    
    /// <summary>
    /// Gets the sub-nodes.
    /// </summary>
    public ObservableCollection<BaseNode> SubNodes { get; }
    
    /// <inheritdoc cref="IsChecked"/>
    private bool _isChecked;
    
    /// <summary>
    /// Gets and sets if this segment is selected for export.
    /// </summary>
    public bool IsChecked
    {
        get => _isChecked;
        set => SetProperty(ref _isChecked, value);
    }
}