using DvdLib.Vmg;

namespace DvdLib.Streams;

public class VideoStream
{
    /// <summary>
    /// Gets the identifier of the video stream file.
    /// </summary>
    public VmgIdentifier Identifier { get; }

    /// <summary>
    /// Gets the IFO file information.
    /// </summary>
    public Ifo Information { get; }

    public VideoStream(VmgIdentifier identifier, Ifo ifo)
    {
        Identifier = identifier;
        Information = ifo;
    }
}