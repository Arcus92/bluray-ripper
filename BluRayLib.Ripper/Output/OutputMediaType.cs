using System.Text.Json.Serialization;

namespace BluRayLib.Ripper.Output;

/// <summary>
/// Defines the type of media for an <see cref="OutputInfo"/>.
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