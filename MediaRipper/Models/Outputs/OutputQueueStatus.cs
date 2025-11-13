using MediaRipper.Services;

namespace MediaRipper.Models.Outputs;

/// <summary>
/// The status of <see cref="OutputQueueService"/>.
/// </summary>
public enum OutputQueueStatus
{
    Idle,
    Running,
}