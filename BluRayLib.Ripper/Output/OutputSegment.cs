using System.Text.Json.Serialization;

namespace BluRayLib.Ripper.Output;

/// <summary>
/// Definition of a segment in an <see cref="OutputInfo"/>.
/// </summary>
[Serializable]
public class OutputSegment
{
    /// <summary>
    /// Gets and sets the segment id.
    /// </summary>
    public ushort Id { get; set; }
}