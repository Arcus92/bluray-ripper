using System.Collections.ObjectModel;
using System.Linq;
using BluRayLib.Ripper.BluRays;

namespace BluRayRipper.Models.Nodes;

public class SegmentNode : BaseNode
{
    public SegmentNode(SegmentData segment)
    {
        Segment = segment;
        
        VideoStreamNode = new TextNode<VideoNode>("Videos")
        {
            IsExpanded = true,
            SubNodes = new ObservableCollection<BaseNode>(Segment.VideoStreams.Select(s => new VideoNode(s) { IsChecked = true }))
        };
        AudioStreamNode = new TextNode<AudioNode>("Audios")
        {
            IsExpanded = true,
            SubNodes = new ObservableCollection<BaseNode>(Segment.AudioStreams.Select(s => new AudioNode(s) { IsChecked = true }))
        };
        SubtitleStreamNode = new TextNode<SubtitleNode>("Subtitles")
        {
            IsExpanded = true,
            SubNodes = new ObservableCollection<BaseNode>(Segment.SubtitleStreams.Select(s => new SubtitleNode(s) { IsChecked = true }))
        };
        SubNodes = [ VideoStreamNode, AudioStreamNode, SubtitleStreamNode ];
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
    /// Gets the video stream sub node.
    /// </summary>
    public TextNode<VideoNode> VideoStreamNode { get; }
    
    /// <summary>
    /// Gets the audio stream sub node.
    /// </summary>
    public TextNode<AudioNode> AudioStreamNode { get; }
    
    /// <summary>
    /// Gets the subtitle stream sub node.
    /// </summary>
    public TextNode<SubtitleNode> SubtitleStreamNode { get; }
    
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