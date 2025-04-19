using System.Runtime.Serialization;

namespace BluRayLib.Ripper.Output;

/// <summary>
/// Defines the type of <see cref="OutputStream"/>.
/// </summary>
public enum OutputStreamType
{
    [EnumMember(Value = "video")]
    Video,
        
    [EnumMember(Value = "audio")]
    Audio,
        
    [EnumMember(Value = "subtitle")]
    Subtitle,
}