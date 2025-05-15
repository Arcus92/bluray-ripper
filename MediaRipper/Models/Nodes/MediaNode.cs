using System.Collections.ObjectModel;
using System.Linq;
using MediaLib.Models;
using MediaLib.Sources;

namespace MediaRipper.Models.Nodes;

public class MediaNode : BaseNode
{
    public MediaNode(IMediaSource source)
    {
        Source = source;

        var segment = Info.Segments.First();
        
        VideoStreamNode = new TextNode<VideoNode>("Videos")
        {
            IsExpanded = true,
            SubNodes = new ObservableCollection<BaseNode>(segment.VideoStreams.Select(s => new VideoNode(s) { IsChecked = true }))
        };
        AudioStreamNode = new TextNode<AudioNode>("Audios")
        {
            IsExpanded = true,
            SubNodes = new ObservableCollection<BaseNode>(segment.AudioStreams.Select(s => new AudioNode(s) { IsChecked = true }))
        };
        SubtitleStreamNode = new TextNode<SubtitleNode>("Subtitles")
        {
            IsExpanded = true,
            SubNodes = new ObservableCollection<BaseNode>(segment.SubtitleStreams.Select(s => new SubtitleNode(s) { IsChecked = true }))
        };
        ChapterNode = new TextNode<ChapterNode>("Chapters")
        {
            IsExpanded = true,
            SubNodes = new ObservableCollection<BaseNode>(Info.Chapters.Select(c => new ChapterNode(c)))
        };
        SubNodes = [ VideoStreamNode, AudioStreamNode, SubtitleStreamNode, ChapterNode ];
    }
    
    /// <summary>
    /// Gets the media source.
    /// </summary>
    public IMediaSource Source { get; }

    /// <summary>
    /// Gets the media info.
    /// </summary>
    public MediaInfo Info => Source.Info;
    
    /// <summary>
    /// Gets the media id.
    /// </summary>
    public ushort Id => Info.Id;
    
    /// <summary>
    /// Gets the title display name.
    /// </summary>
    public string DisplayName => Info.ToString();
    
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