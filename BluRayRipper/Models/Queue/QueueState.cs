namespace BluRayRipper.Models.Queue;

/// <summary>
/// The state of <see cref="QueuedTask"/>.
/// </summary>
public enum QueueState
{
    Ready,
    Running,
    Finished,
    Failed,
}