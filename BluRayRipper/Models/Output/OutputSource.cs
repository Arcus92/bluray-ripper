using System.Runtime.Serialization;

namespace BluRayRipper.Models.Output;

public enum OutputSource
{
    [EnumMember(Value = "BluRay")]
    BluRay,
    
    [EnumMember(Value = "DVD")]
    Dvd
}