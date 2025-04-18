using System.Text.Json.Serialization;

namespace BluRayLib.Ripper;

[Serializable]
public class CodecOptions
{
    /// <summary>
    /// Gets and sets the FFmpeg video codec.
    /// </summary>
    public string VideoCodec { get; set; } = "copy";
    
    /// <summary>
    /// Gets and sets the FFmpeg audio codec.
    /// </summary>
    public string AudioCodec { get; set; } = "copy";
    
    /// <summary>
    /// Gets and sets the FFmpeg subtitle codec.
    /// </summary>
    public string SubtitleCodec { get; set; } = "copy";
    
    /// <summary>
    /// Gets and sets the FFmpeg constant rate factor.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? ConstantRateFactor  { get; set; }
    
    /// <summary>
    /// Gets and sets the FFmpeg max bitrate.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? MaxRate  { get; set; }
    
    /// <summary>
    /// Gets and sets the FFmpeg buffer size.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? BufferSize  { get; set; }
}