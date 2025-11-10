using System.Text.Json.Serialization;

namespace MediaLib.Output;

/// <summary>
/// Defines additional flags of <see cref="OutputStream"/>.
/// </summary>
[Flags]
[JsonConverter(typeof(JsonStringEnumConverter<OutputStreamFlags>))]
public enum OutputStreamFlags
{
    None = 0,
    Default = 1 << 0,
    Secondary = 1 << 1,
    Forced = 1 << 2
}