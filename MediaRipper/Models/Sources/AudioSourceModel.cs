using MediaLib.Models;

namespace MediaRipper.Models.Sources;

public class AudioSourceModel : StreamSourceModel
{
    public AudioSourceModel(AudioInfo stream) : base(stream)
    {
        Stream = stream;
    }

    /// <summary>
    /// Gets the stream info.
    /// </summary>
    public new AudioInfo Stream { get; }
}