using System.Linq;
using BluRayLib.Ripper;
using BluRayLib.Ripper.BluRays;
using BluRayLib.Ripper.Output;

namespace BluRayRipper.Models.Nodes;

public static class Mapper
{
    /// <summary>
    /// Creates an output info object from the given title nodes with the given codec and format.
    /// </summary>
    /// <param name="node">The title node.</param>
    /// <param name="codec">The codec options.</param>
    /// <param name="format">The video format.</param>
    /// <param name="diskInfo">The disk info.</param>
    /// <returns></returns>
    public static OutputInfo ToOutputInfo(this TitleNode node, CodecOptions codec, VideoFormat format,
        DiskInfo diskInfo)
    {
        var output = node.Playlist.ToOutputInfo(codec, format, diskInfo);

        // Map the node ui to the output settings.
        foreach (var file in output.Files)
        {
            foreach (var stream in file.Streams)
            {
                stream.Enabled = node.IsStreamChecked(stream.Id);
            }
        }
        
        return output;
    }

    /// <summary>
    /// Gets if the stream with the given id is checked for export in the view node.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <param name="streamId">The stream id.</param>
    /// <returns>Returns if the node was checked and enabled for export.</returns>
    private static bool IsStreamChecked(this TitleNode node, ushort streamId)
    {
        var segment = node.SegmentNode.Items.First();
        foreach (var stream in segment.StreamNodes)
        {
            if (stream.Id == streamId)
                return stream.IsChecked;
        }
        return false;
    }
}