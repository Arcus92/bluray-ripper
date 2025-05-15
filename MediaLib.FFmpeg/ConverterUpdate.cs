namespace MediaLib.FFmpeg;

/// <summary>
/// Data for the FFmpeg converter update.
/// </summary>
public struct ConverterUpdate
{
    /// <summary>
    /// Gets the inputs to convert.
    /// </summary>
    public List<InputMetadata> Inputs { get; set; }

    /// <summary>
    /// Gets the duration of the input stream.
    /// </summary>
    public TimeSpan? Duration { get; set; }

    /// <summary>
    /// Gets the current frame position of the converter.
    /// </summary>
    public TimeSpan? Current { get; set; }

    /// <summary>
    /// Gets the current frame of the converter.
    /// </summary>
    public long? Frame { get; set; }

    /// <summary>
    /// Gets the percentage of the converter.
    /// </summary>
    public double? Percentage { get; set; }
}