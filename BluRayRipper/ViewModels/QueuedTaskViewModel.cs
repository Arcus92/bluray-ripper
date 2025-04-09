using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using BluRayRipper.Models.Queue;
using BluRayRipper.Views;

namespace BluRayRipper.ViewModels;

public class QueuedTaskViewModel : ViewModelBase
{
    /// <summary>
    /// Gets the queued task object.
    /// </summary>
    public QueuedTask QueuedTask { get; }

    public QueuedTaskViewModel(QueuedTask queuedTask)
    {
        QueuedTask = queuedTask;
        QueuedTask.ProgressChanged += OnProgressChanged;
    }
    
    // Designer default
    public QueuedTaskViewModel() : this(new QueuedTask("Example task", _ => Task.CompletedTask))
    {
    }

    public string Name => QueuedTask.Name;
    
    public double Progress => QueuedTask.Progress;

    private void OnProgressChanged(object? sender, EventArgs e)
    {
        OnPropertyChanged(nameof(Progress));
    }
    
    /// <inheritdoc />
    public override Control CreateView()
    {
        return new QueuedTaskView();
    }
}