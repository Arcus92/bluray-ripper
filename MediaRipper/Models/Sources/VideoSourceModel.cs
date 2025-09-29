using MediaLib.Models;

namespace MediaRipper.Models.Sources;

public class VideoSourceModel : StreamSourceModel
{
    public VideoSourceModel(VideoInfo stream) : base(stream)
    {
        Stream = stream;
    }

    /// <summary>
    /// Gets the stream info.
    /// </summary>
    public new VideoInfo Stream { get; }
}