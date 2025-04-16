using System;
using System.Text.Json.Serialization;

namespace BluRayRipper.Models.Output;

[Serializable]
public class OutputStream
{
    /// <summary>
    /// Gets and sets the stream id.
    /// </summary>
    public ushort Id { get; set; }

    /// <summary>
    /// Gets and sets if this is the default track.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool Default { get; set; }

    /// <summary>
    /// Gets and sets the external filename of this stream. Only set if this stream was exported as a separate file.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Extension { get; set; }
    
    /// <summary>
    /// Gets and sets the language code of this stream.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? LanguageCode { get; set; }

    /// <summary>
    /// Gets if this is an external file.
    /// </summary>
    [JsonIgnore]
    public bool IsExternal => Extension is not null;
}