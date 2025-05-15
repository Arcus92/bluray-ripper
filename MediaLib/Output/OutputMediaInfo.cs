using System.Text.Json.Serialization;

namespace MediaLib.Output;

[Serializable]
public class OutputMediaInfo
{
    /// <summary>
    /// Gets and sets the media type.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public OutputMediaType Type { get; set; } = OutputMediaType.Unset;

    /// <summary>
    /// Gets and sets the name of this file.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string Name { get; set; } = "";
    
    /// <summary>
    /// Gets and sets the season.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Season { get; set; }
    
    /// <summary>
    /// Gets and sets the episode.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Episode { get; set; }
    
    /// <summary>
    /// Gets and sets the IMDB id.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? ImdbId { get; set; }
}