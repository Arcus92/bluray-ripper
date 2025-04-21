using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BluRayLib.Ripper;
using BluRayLib.Ripper.BluRays;
using BluRayLib.Ripper.BluRays.Export;
using BluRayLib.Ripper.Output;
using BluRayRipper.Models;
using BluRayRipper.Models.Output;
using BluRayRipper.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace BluRayRipper.Services;

public class OutputQueueService(
    ILogger<OutputQueueService> logger,
    IDiskService diskService,
    IOutputService outputService)
    : IOutputQueueService
{
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
        foreach (var output in outputService.Outputs)
        {
            if (output.Status == OutputStatus.Completed) continue;
            if (output.Info.Source.DiskName != diskService.DiskName) continue;
            _queue.Add(output);
        }
        
        // Start the thread
        _isRunning = true;
        await Task.Run(async () =>
        {
            var exporter = diskService.CreateTitleExporter();

            foreach (var model in _queue)
            {
                try
                {
                    var outputPath = outputService.OutputPath;
                    
                    model.Status = OutputStatus.Running;
                    await exporter.ExportAsync(outputPath, model.Info,
                        onUpdate: update => { model.Progress = update.Percentage ?? 0.0; });
                    model.Progress = 1.0;
                    
                    // Updates the status
                    outputService.UpdateStatus(model);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed FFmpeg export for playlist {PlaylistId:00000}!", model.Info.Source.PlaylistId);
                    model.Status = OutputStatus.Failed;
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