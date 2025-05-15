using System.Text.Json.Serialization;

namespace MediaLib.Output;

/// <summary>
/// Defines a stream in an <see cref="OutputFile"/>.
/// </summary>
[Serializable]
public class OutputStream
{
    /// <summary>
    /// Gets and sets the stream id.
    /// </summary>
    public ushort Id { get; set; }
    
    /// <summary>
    /// Gets and sets the stream type.
    /// </summary>
    public OutputStreamType Type { get; set; }
    
    /// <summary>
    /// Gets and sets if this stream is enabled for export.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets and sets if this is the default track.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool Default { get; set; }
    
    /// <summary>
    /// Gets and sets the language code of this stream.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? LanguageCode { get; set; }
}