using System;
using System.Collections.Generic;
using BluRayRipper.Models.Queue;

namespace BluRayRipper.Services.Interfaces;

public interface IQueueService
{
    /// <summary>
    /// Gets the list of queued tasks.
    /// </summary>
    IReadOnlyList<QueuedTask> Tasks { get; }
    
    /// <summary>
    /// The event that is invoked when a new task was added.
    /// </summary>
    event EventHandler<QueuedTask> TaskAdded;
    
    /// <summary>
    /// The event that is invoked when task was removed.
    /// </summary>
    event EventHandler<QueuedTask> TaskRemoved;
    
    /// <summary>
    /// Adds the task to the queue.
    /// </summary>
    /// <param name="queuedTask">The task.</param>
    void QueueTask(QueuedTask queuedTask);
}