using BluRayLib.Ripper.BluRays;

namespace BluRayRipper.Models.Nodes;

public class SubtitleNode : StreamNode
{
    public SubtitleNode(SubtitleData stream) : base(stream)
    {
        Stream = stream;
    }

    /// <summary>
    /// Gets the stream info.
    /// </summary>
    public new SubtitleData Stream { get; }
}