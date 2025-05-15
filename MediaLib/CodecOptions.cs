using System.Text.Json.Serialization;

namespace MediaLib;

[Serializable]
public readonly struct CodecOptions
{
    public CodecOptions()
    {
    }

    /// <summary>
    /// Gets and sets the FFmpeg video codec.
    /// </summary>
    public string VideoCodec { get; init; } = "copy";
    
    /// <summary>
    /// Gets and sets the FFmpeg audio codec.
    /// </summary>
    public string AudioCodec { get; init; } = "copy";
    
    /// <summary>
    /// Gets and sets the FFmpeg subtitle codec.
    /// </summary>
    public string SubtitleCodec { get; init; } = "copy";
    
    /// <summary>
    /// Gets and sets the FFmpeg constant rate factor.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? ConstantRateFactor { get; init; }  = null;
    
    /// <summary>
    /// Gets and sets the FFmpeg max bitrate.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? MaxRate { get; init; } = null;

    /// <summary>
    /// Gets and sets the FFmpeg buffer size.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? BufferSize { get; init; } = null;
}