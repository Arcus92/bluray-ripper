using System.Text.Json.Serialization;

namespace MediaLib.Output;

/// <summary>
/// Defines the type of <see cref="OutputStream"/>.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<OutputStreamType>))]
public enum OutputStreamType
{
    Video,
    Audio,
    Subtitle,
}