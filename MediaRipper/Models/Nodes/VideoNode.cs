using MediaLib.Models;

namespace MediaRipper.Models.Nodes;

public class VideoNode : StreamNode
{
    public VideoNode(VideoInfo stream) : base(stream)
    {
        Stream = stream;
    }

    /// <summary>
    /// Gets the stream info.
    /// </summary>
    public new VideoInfo Stream { get; }
}