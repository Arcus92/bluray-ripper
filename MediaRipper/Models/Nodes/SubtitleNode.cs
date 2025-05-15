using MediaLib.Models;

namespace MediaRipper.Models.Nodes;

public class SubtitleNode : StreamNode
{
    public SubtitleNode(SubtitleInfo stream) : base(stream)
    {
        Stream = stream;
    }

    /// <summary>
    /// Gets the stream info.
    /// </summary>
    public new SubtitleInfo Stream { get; }
}