using BluRayLib.Ripper.BluRays;

namespace BluRayRipper.Models.Nodes;

public class VideoNode : StreamNode
{
    public VideoNode(VideoData stream) : base(stream)
    {
        Stream = stream;
    }

    /// <summary>
    /// Gets the stream info.
    /// </summary>
    public new VideoData Stream { get; }
}