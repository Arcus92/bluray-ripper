using System.Text.Json.Serialization;

namespace BluRayLib.Ripper.Output;

/// <summary>
/// Defines the source of an <see cref="OutputInfo"/>.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<OutputSourceType>))]
public enum OutputSourceType
{
    BluRay,
    Dvd,
    File,
}