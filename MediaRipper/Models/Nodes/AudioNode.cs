using MediaLib.Models;

namespace MediaRipper.Models.Nodes;

public class AudioNode : StreamNode
{
    public AudioNode(AudioInfo stream) : base(stream)
    {
        Stream = stream;
    }

    /// <summary>
    /// Gets the stream info.
    /// </summary>
    public new AudioInfo Stream { get; }
}