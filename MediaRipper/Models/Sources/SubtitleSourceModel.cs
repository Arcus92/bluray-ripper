using MediaLib.Models;

namespace MediaRipper.Models.Sources;

public class SubtitleSourceModel : StreamSourceModel
{
    public SubtitleSourceModel(SubtitleInfo stream) : base(stream)
    {
        Stream = stream;
    }

    /// <summary>
    /// Gets the stream info.
    /// </summary>
    public new SubtitleInfo Stream { get; }
}