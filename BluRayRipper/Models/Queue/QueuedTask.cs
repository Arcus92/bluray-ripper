using System;
using System.Threading.Tasks;

namespace BluRayRipper.Models.Queue;

public class QueuedTask
{
    public QueuedTask(string name, Func<QueuedTask, Task> startFunc)
    {
        Name = name;
        StartFunc = startFunc;
    }

    /// <summary>
    /// Gets the name of the task.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the task start function.
    /// </summary>
    public Func<QueuedTask, Task> StartFunc { get; }

    private double _progress;

    /// <summary>
    /// Gets the current progress of the task.
    /// </summary>
    public double Progress
    {
        get => _progress;
        set
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (_progress == value) return;
            _progress = value;
            ProgressChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    
    /// <summary>
    /// The event that is invoked when the progress value was changed.
    /// </summary>
    public event EventHandler? ProgressChanged; 
}