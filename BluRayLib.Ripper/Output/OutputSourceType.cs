using System.Runtime.Serialization;

namespace BluRayLib.Ripper.Output;

/// <summary>
/// Defines the source of an <see cref="OutputInfo"/>.
/// </summary>
public enum OutputSourceType
{
    [EnumMember(Value = "bluray")]
    BluRay,
    
    [EnumMember(Value = "dvd")]
    Dvd,
    
    [EnumMember(Value = "file")]
    File,
}