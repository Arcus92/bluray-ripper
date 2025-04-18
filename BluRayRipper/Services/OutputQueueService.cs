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
    /// The output queue.
    /// </summary>
    private readonly List<OutputModel> _queue = [];
    
    /// <summary>
    /// Gets and sets if the queue is currently running.
    /// </summary>
    private bool _isRunning;

    /// <inheritdoc />
    public void Start()
    {
        if (_isRunning)
            return;
        
        // Build the initial progress map
        _queue.Clear();
        foreach (var output in _outputService.Items)
        {
            if (output.Status == OutputStatus.Completed) continue;
            if (output.DiskName != _diskService.DiskName) continue;
            _queue.Add(output);
        }
        
        // Start the thread
        _isRunning = true;
        Task.Run(async () =>
        {
            var exporter = _diskService.CreateTitleExporter();

            foreach (var output in _queue)
            {
                var options = _outputService.BuildExportOptionsFormOutputInfo(output.File);
                var outputPath = _outputService.OutputPath;

                output.Status = OutputStatus.Running;
                await exporter.ExportAsync(outputPath, options, onUpdate: update =>
                {
                    output.Progress = update.Percentage ?? 0.0;
                });
                output.Progress = 1.0;
                output.Status = OutputStatus.Completed;
            }

            _isRunning = false;
        });
    }

    /// <inheritdoc />
    public void Stop()
    {
        throw new NotImplementedException();
    }
}