namespace BluRayRipper.Models.Output;

/// <summary>
/// The output status.
/// </summary>
public enum OutputStatus
{
    /// <summary>
    /// The export is queued.
    /// </summary>
    Queued,
    
    /// <summary>
    /// The export is running.
    /// </summary>
    Running,
    
    /// <summary>
    /// The export was completed.
    /// </summary>
    Completed,
    
    /// <summary>
    /// The export has failed.
    /// </summary>
    Failed,
    
    /// <summary>
    /// The export is queued, but the current disk doesn't match.
    /// </summary>
    QueuedMismatchDisk,
}