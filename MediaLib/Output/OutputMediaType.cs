using System.Text.Json.Serialization;

namespace MediaLib.Output;

/// <summary>
/// Defines the type of media for an <see cref="OutputDefinition"/>.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<OutputMediaType>))]
public enum OutputMediaType
{
    Unset,
    Movie,
    Episode,
    Extra,
    MakingOf,
    BehindTheScenes,
    Interview,
    Trailer,
}