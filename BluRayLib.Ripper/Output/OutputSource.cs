using System.Text.Json.Serialization;

namespace BluRayLib.Ripper.Output;

/// <summary>
/// Defines the source of the output file.
/// </summary>
[Serializable]
public class OutputSource
{
    /// <summary>
    /// Gets and sets the source type.
    /// </summary>
    public OutputSourceType Type { get; set; }
    
    /// <summary>
    /// Gets and sets the disk name from which this output file was generated.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string DiskName { get; set; } = "";
    
    /// <summary>
    /// Gets and sets the content hash of the source.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string ContentHash { get; set; } = "";
    
    /// <summary>
    /// Gets and sets the playlist id from which this output file was generated.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ushort PlaylistId { get; set; }

    /// <summary>
    /// Gets and sets the segments.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public OutputSegment[] Segments { get; set; } = [];
}