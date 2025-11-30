using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using FluentIcons.Common;
using MediaLib;
using MediaLib.Models;
using MediaLib.Sources;

namespace MediaRipper.Models.Sources;

public class MediaSourceModel : BaseSourceModel
{
    public MediaSourceModel(IMediaSource source)
    {
        Source = source;

        var segment = Info.Segments.First();
        
        VideoStreamNode = new TextSourceModel<VideoSourceModel>("Videos")
        {
            IsExpanded = true,
            SubNodes = new ObservableCollection<BaseSourceModel>(segment.VideoStreams.Select(s => new VideoSourceModel(s)))
        };
        AudioStreamNode = new TextSourceModel<AudioSourceModel>("Audios")
        {
            IsExpanded = true,
            SubNodes = new ObservableCollection<BaseSourceModel>(segment.AudioStreams.Select(s => new AudioSourceModel(s)))
        };
        SubtitleStreamNode = new TextSourceModel<SubtitleSourceModel>("Subtitles")
        {
            IsExpanded = true,
            SubNodes = new ObservableCollection<BaseSourceModel>(segment.SubtitleStreams.Select(s => new SubtitleSourceModel(s)))
        };
        ChapterNode = new TextSourceModel<ChapterSourceModel>("Chapters")
        {
            IsExpanded = true,
            SubNodes = new ObservableCollection<BaseSourceModel>(Info.Chapters.Select(c => new ChapterSourceModel(c)))
        };
        SubNodes = [ VideoStreamNode, AudioStreamNode, SubtitleStreamNode, ChapterNode ];
        
        // Build the segment description
        SegmentDescriptionText = BuildSegmentDescription(Info.Segments);
        Icons = BuildMediaIcons(Source.IgnoreFlags);
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
    /// Gets the icons displayed next the media item.
    /// </summary>
    public MediaSourceIconModel[] Icons { get; }

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
    public TextSourceModel<VideoSourceModel> VideoStreamNode { get; }
    
    /// <summary>
    /// Gets the audio stream sub node.
    /// </summary>
    public TextSourceModel<AudioSourceModel> AudioStreamNode { get; }
    
    /// <summary>
    /// Gets the subtitle stream sub node.
    /// </summary>
    public TextSourceModel<SubtitleSourceModel> SubtitleStreamNode { get; }
    
    /// <summary>
    /// Gets the chapter category node.
    /// </summary>
    public TextSourceModel<ChapterSourceModel> ChapterNode { get; }
    
    /// <summary>
    /// Gets the sub-nodes.
    /// </summary>
    public ObservableCollection<TextSourceModel> SubNodes { get; }
    
    
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
    private static string BuildSegmentDescription(SegmentInfo[] segments)
    {
        // Some sources don't have relevant segment ids.
        if (segments is [{ Id: 0 }])
        {
            return "";
        }
        
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

    /// <summary>
    /// Builds the list of icons attached to the source.
    /// </summary>
    /// <param name="ignoreFlags">The media ignore flags to convert to icons.</param>
    /// <returns>Returns the list of icons.</returns>
    private static MediaSourceIconModel[] BuildMediaIcons(MediaIgnoreFlags ignoreFlags)
    {
        var list = new List<MediaSourceIconModel>();

        if ((ignoreFlags & MediaIgnoreFlags.TooShort) != 0)
        {
            list.Add(new MediaSourceIconModel(Icon.Clock, "Too short"));
        }
        if ((ignoreFlags & MediaIgnoreFlags.TooLong) != 0)
        {
            list.Add(new MediaSourceIconModel(Icon.Clock, "Too long"));
        }
        if ((ignoreFlags & MediaIgnoreFlags.NoSubtitle) != 0)
        {
            list.Add(new MediaSourceIconModel(Icon.TextStrikethrough, "No subtitles"));
        }
        if ((ignoreFlags & MediaIgnoreFlags.NoAudio) != 0)
        {
            list.Add(new MediaSourceIconModel(Icon.SpeakerMute, "No audio"));
        }
        if ((ignoreFlags & MediaIgnoreFlags.Menu) != 0)
        {
            list.Add(new MediaSourceIconModel(Icon.PanelLeftText, "Menu"));
        }
        if ((ignoreFlags & MediaIgnoreFlags.Duplicate) != 0)
        {
            list.Add(new MediaSourceIconModel(Icon.Copy, "Duplicate"));
        }
        if ((ignoreFlags & MediaIgnoreFlags.RepeatingClips) != 0)
        {
            list.Add(new MediaSourceIconModel(Icon.ArrowRepeatAll, "Repeating clips"));
        }
        
        return list.ToArray();
    }
}