using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BluRayRipper.Models.Output;
using BluRayRipper.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace BluRayRipper.Services;

public class OutputQueueService : IOutputQueueService
{
    private readonly ILogger<OutputQueueService> _logger;
    private readonly IDiskService _diskService;
    private readonly IOutputService _outputService;
    
    public OutputQueueService(ILogger<OutputQueueService> logger, IDiskService diskService, IOutputService outputService)
    {
        _logger = logger;
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
    public async Task StartAsync()
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
        await Task.Run(async () =>
        {
            var exporter = _diskService.CreateTitleExporter();

            foreach (var output in _queue)
            {
                try
                {
                    var options = _outputService.BuildExportOptionsFormOutputInfo(output.File);
                    var outputPath = _outputService.OutputPath;
                    
                    output.Status = OutputStatus.Running;
                    await exporter.ExportAsync(outputPath, options,
                        onUpdate: update => { output.Progress = update.Percentage ?? 0.0; });
                    output.Progress = 1.0;
                    output.Status = OutputStatus.Completed;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed FFmpeg export for playlist {PlaylistId:00000}!", output.PlaylistId);
                    output.Status = OutputStatus.Failed;
                }
            }

            _isRunning = false;
        });
    }

    /// <inheritdoc />
    public Task StopAsync()
    {
        throw new NotImplementedException();
    }
}