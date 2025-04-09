using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BluRayRipper.Models.Queue;
using BluRayRipper.Services.Interfaces;

namespace BluRayRipper.Services;

public class QueueService : IQueueService
{
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

    private QueuedTask? _currentQueuedTask;
    private Task? _currentTask;
    
    private void CheckStartNextTask()
    {
        if (_currentQueuedTask is not null) return;
        if (Tasks.Count == 0) return;
        
        var queuedTask = Tasks[0];
        _currentQueuedTask = queuedTask;
        
        Task.Run(async () =>
        {
            // TODO: Make this thread safe!
            
            var task = queuedTask.StartFunc(queuedTask);
            _currentTask = task;
            
            await task.ConfigureAwait(true);
            
            _currentQueuedTask = null;
            _currentTask = null;

            _tasks.Remove(queuedTask);
            TaskRemoved?.Invoke(this, queuedTask);
            CheckStartNextTask();
        });
    }
}