using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Avalonia.Threading;
using BluRayRipper.Models.Output;
using BluRayRipper.Services.Interfaces;

namespace BluRayRipper.Services;

public class OutputQueueService : IOutputQueueService
{
    private readonly IDiskService _diskService;
    private readonly IOutputService _outputService;
    
    public OutputQueueService(IDiskService diskService, IOutputService outputService)
    {
        _diskService = diskService;
        _outputService = outputService;
    }
    
    /// <summary>
    /// The list containing a map of all queued outputs and their current progress objects, if queued.
    /// </summary>
    private readonly Dictionary<OutputFile, OutputProgress> _outputToProgress = new();
    
    /// <summary>
    /// Gets and sets if the queue is currently running.
    /// </summary>
    private bool _isRunning;
    
    /// <inheritdoc />
    public void Clear()
    {
        Stop();
        _outputToProgress.Clear();
    }

    /// <inheritdoc />
    public void Start()
    {
        if (_isRunning)
            return;
        
        // Build the initial progress map
        _outputToProgress.Clear();
        foreach (var output in _outputService.Items)
        {
            if (output.Status == OutputFileStatus.Completed) continue;
            _outputToProgress.Add(output, new OutputProgress());
        }
        
        // Start the thread
        _isRunning = true;
        Task.Run(async () =>
        {
            await InvokeQueueProgressChangedAsync();
            
            var exporter = _diskService.CreateTitleExporter();

            foreach (var (output, progress) in _outputToProgress)
            {
                var options = _outputService.BuildExportOptionsFormOutputInfo(output);
                var outputPath = _outputService.OutputPath;

                output.Status = OutputFileStatus.Running;
                await _outputService.UpdateAsync(output);

                await InvokeQueueProgressChangedAsync();
                
                await exporter.ExportAsync(outputPath, options, onUpdate: update =>
                {
                    progress.Progress = update.Percentage ?? 0.0;
                });
                progress.Progress = 1.0;
                
                output.Status = OutputFileStatus.Completed;
                await _outputService.UpdateAsync(output);

                await InvokeQueueProgressChangedAsync();
            }

            _isRunning = false;
        });
    }

    /// <inheritdoc />
    public void Stop()
    {
        throw new System.NotImplementedException();
    }

    /// <inheritdoc />
    public event EventHandler? QueueProgressChanged;

    private async Task InvokeQueueProgressChangedAsync()
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            QueueProgressChanged?.Invoke(this, EventArgs.Empty);
        });
    }

    /// <inheritdoc />
    public bool TryGetProcess(OutputFile output, [MaybeNullWhen(false)] out OutputProgress progress)
    {
        return _outputToProgress.TryGetValue(output, out progress);
    }
}