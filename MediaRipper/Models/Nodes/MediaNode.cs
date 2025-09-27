using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using MediaLib;
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
            SubNodes = new ObservableCollection<BaseNode>(segment.VideoStreams.Select(s => new VideoNode(s)))
        };
        AudioStreamNode = new TextNode<AudioNode>("Audios")
        {
            IsExpanded = true,
            SubNodes = new ObservableCollection<BaseNode>(segment.AudioStreams.Select(s => new AudioNode(s)))
        };
        SubtitleStreamNode = new TextNode<SubtitleNode>("Subtitles")
        {
            IsExpanded = true,
            SubNodes = new ObservableCollection<BaseNode>(segment.SubtitleStreams.Select(s => new SubtitleNode(s)))
        };
        ChapterNode = new TextNode<ChapterNode>("Chapters")
        {
            IsExpanded = true,
            SubNodes = new ObservableCollection<BaseNode>(Info.Chapters.Select(c => new ChapterNode(c)))
        };
        SubNodes = [ VideoStreamNode, AudioStreamNode, SubtitleStreamNode, ChapterNode ];
        
        // Build the segment description
        SegmentDescriptionText = BuildSegmentDescription(Info.Segments);
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
    /// Gets the media ignore flags.
    /// </summary>
    public MediaIgnoreFlags IgnoreFlags => Source.IgnoreFlags;

    /// <summary>
    /// Gets the segment usage text.
    /// </summary>
    public string SegmentDescriptionText { get; }

    /// <summary>
    /// Gets the media id.
    /// </summary>
    public ushort Id => Info.Id;
    
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

    /// <summary>
    /// Builds a readable segment description. This will also count multiple segments in succession.
    /// For example, instead of repeating '1, 1, 1, 1' it will print '4x1' instead.
    /// </summary>
    /// <returns>Returns a string representation of the segment ids.</returns>
    private static string BuildSegmentDescription(IEnumerable<SegmentInfo> segments)
    {
        var builder = new StringBuilder();

        var counter = 0;
        ushort previousId = 0;

        foreach (var segment in segments)
        {
            if (segment.Id == previousId)
            {
                counter++;
            }
            else
            {
                AddSegmentId();
                previousId = segment.Id;
                counter = 1;
            }
        }

        AddSegmentId();

        return builder.ToString();

        void AddSegmentId()
        {
            if (counter == 0) return;
            if (builder.Length > 0) builder.Append(", ");
            if (counter > 1)
            {
                builder.Append(counter);
                builder.Append('x');
            }
            builder.Append(previousId);
        }
    }
}