using MediaLib.Models;
using MediaLib.Output;

namespace MediaLib.Sources;

public interface IMediaSource
{
    /// <summary>
    /// Gets the media identifier.
    /// </summary>
    MediaIdentifier Identifier { get; }

    /// <summary>
    /// Gets the media information.
    /// </summary>
    MediaInfo Info { get; }
    
    /// <summary>
    /// Gets the ignore flags of this media.
    /// </summary>
    MediaIgnoreFlags IgnoreFlags { get; }
    
    /// <summary>
    /// Creates a default output definition for this media source.
    /// </summary>
    /// <param name="codec">The codec options to use.</param>
    /// <param name="format">The video format to use.</param>
    /// <returns>Returns the output definition.</returns>
    OutputDefinition CreateDefaultOutputDefinition(CodecOptions codec, VideoFormat format);
}