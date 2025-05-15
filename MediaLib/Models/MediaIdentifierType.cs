using System.Text.Json.Serialization;

namespace MediaLib.Models;

/// <summary>
/// Defines the type of <see cref="MediaIdentifier"/>.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<MediaIdentifierType>))]
public enum MediaIdentifierType
{
    BluRay,
    Dvd,
    File,
}