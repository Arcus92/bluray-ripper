namespace BluRayRipper.Models.Output;

/// <summary>
/// The output status.
/// </summary>
public enum OutputFileStatus
{
    /// <summary>
    /// The output is queued.
    /// </summary>
    Queued,
    
    /// <summary>
    /// The output export is running.
    /// If an output info file was loaded with this state, it is likely that the export was interrupted.
    /// </summary>
    Running,
    
    /// <summary>
    /// The output was completely processed.
    /// </summary>
    Completed,
}