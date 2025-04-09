using System.Collections.ObjectModel;
using Avalonia.Controls;
using BluRayRipper.Models.Queue;
using BluRayRipper.Services;
using BluRayRipper.Services.Interfaces;
using BluRayRipper.Views;

namespace BluRayRipper.ViewModels;

public class OutputViewModel : ViewModelBase
{
    private readonly IQueueService _queue;

    public OutputViewModel(IQueueService queue)
    {
        _queue = queue;
        _queue.TaskAdded += QueueTaskAdded;
        _queue.TaskRemoved += QueueOnTaskRemoved;
        
        foreach (var task in _queue.Tasks)
        {
            Tasks.Add(new QueuedTaskViewModel(task));
        }
    }
    
    // Designer default
    public OutputViewModel() : this(new QueueService())
    {
    }

    /// <summary>
    /// Gets the task list.
    /// </summary>
    public ObservableCollection<QueuedTaskViewModel> Tasks { get; } = [];
    
    
    private void QueueTaskAdded(object? sender, QueuedTask e)
    {
        Tasks.Add(new QueuedTaskViewModel(e));
    }
    private void QueueOnTaskRemoved(object? sender, QueuedTask e)
    {
        foreach (var task in Tasks)
        {
            if (task.QueuedTask != e) continue;
            Tasks.Remove(task);
            break;
        }
    }
    

    /// <inheritdoc />
    public override Control CreateView()
    {
        return new OutputView();
    }
}