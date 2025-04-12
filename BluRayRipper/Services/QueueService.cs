using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BluRayRipper.Models.Queue;
using BluRayRipper.Services.Interfaces;
using BluRayRipper.Utils;
using Microsoft.Extensions.Logging;

namespace BluRayRipper.Services;

/// <summary>
/// The service to queue async tasks like exporting.
/// </summary>
public class QueueService : IQueueService
{
    private readonly ILogger<QueueService> _logger;
    
    public QueueService(ILogger<QueueService> logger)
    {
        _logger = logger;
    }

    public QueueService() : this(EmptyLogger<QueueService>.Create())
    {
    }
    
    /// <summary>
    /// The current task queue.
    /// </summary>
    private readonly List<QueuedTask> _tasks = [];
    
    /// <inheritdoc />
    public IReadOnlyList<QueuedTask> Tasks => _tasks.AsReadOnly();
    
    /// <inheritdoc />
    public event EventHandler<QueuedTask>? TaskAdded;
    
    /// <inheritdoc />
    public event EventHandler<QueuedTask>? TaskRemoved;
    
    /// <inheritdoc />
    public void QueueTask(QueuedTask queuedTask)
    {
        _tasks.Add(queuedTask);
        TaskAdded?.Invoke(this, queuedTask);
        CheckStartNextTask();
    }

    /// <summary>
    /// The current running queued task.
    /// </summary>
    private QueuedTask? _currentQueuedTask;
    
    /// <summary>
    /// The current running task.
    /// </summary>
    private Task? _currentTask;

    private QueuedTask? GetNextReadyTask()
    {
        foreach (var task in _tasks)
        {
            if (task.State == QueueState.Ready)
                return task;
        }
        return null;
    }
    
    private void CheckStartNextTask()
    {
        if (_currentQueuedTask is not null) return;
        var queuedTask = GetNextReadyTask();
        if (queuedTask is null) return;
        _currentQueuedTask = queuedTask;
        queuedTask.State = QueueState.Running;
        
        Task.Run(async () =>
        {
            // TODO: Make this thread safe!
            
            _logger.LogInformation("Starting queued task: {TaskName}", queuedTask.Name);
            
            var task = queuedTask.StartFunc(queuedTask);
            _currentTask = task;

            try
            {
                
                await task.ConfigureAwait(true);
                queuedTask.State = QueueState.Finished;
                
                _logger.LogInformation("Queued task finished: {TaskName}", queuedTask.Name);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Queued task failed: {TaskName}", queuedTask.Name);
                queuedTask.State = QueueState.Failed;
            }
            
            _currentQueuedTask = null;
            _currentTask = null;
            
            CheckStartNextTask();
        });
    }
}