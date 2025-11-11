using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediaLib.Providers;
using MediaRipper.Models.Outputs;
using MediaRipper.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace MediaRipper.Services;

public class OutputQueueService(
    ILogger<OutputQueueService> logger,
    IMediaProviderService mediaProviderService,
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
            if (!mediaProviderService.Contains(output.Definition.Identifier)) continue;
            _queue.Add(output);
        }
        
        // Start the thread
        _isRunning = true;
        await Task.Run(async () =>
        {
            foreach (var model in _queue)
            {
                try
                {
                    var outputPath = outputService.OutputPath;
                    var parameter = new MediaConverterParameter(
                        outputPath, 
                        model.Definition, 
                        onUpdate: update => { model.Progress = update.Percentage ?? 0.0; });
                    
                    var converter = mediaProviderService.CreateConverter(parameter);
                    
                    model.Status = OutputStatus.Processing;
                    await converter.ExecuteAsync();
                    model.Progress = 1.0;
                    
                    // Updates the status
                    outputService.UpdateStatus(model);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed FFmpeg export for playlist {PlaylistId:00000}!", model.Definition.Identifier.Id);
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