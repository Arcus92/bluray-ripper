using System;
using System.Collections.Generic;
using System.Threading;
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

    /// <see cref="Status"/>
    private OutputQueueStatus _status;

    /// <inheritdoc />
    public OutputQueueStatus Status
    {
        get => _status;
        private set
        {
            if (_status == value) return;
            _status = value;
            StatusChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <inheritdoc />
    public event EventHandler? StatusChanged;


    private CancellationTokenSource? _cancellationTokenSource;
    
    /// <inheritdoc />
    public void Start()
    {
        if (Status != OutputQueueStatus.Idle)
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
        Status = OutputQueueStatus.Running;
        _cancellationTokenSource = new CancellationTokenSource();
        
        var cancellationToken = _cancellationTokenSource.Token;
        Task.Run(async () =>
        {
            foreach (var model in _queue)
            {
                try
                {
                    if (cancellationToken.IsCancellationRequested) break;
                    
                    var outputPath = outputService.OutputPath;
                    var parameter = new MediaConverterParameter(
                        outputPath, 
                        model.Definition, 
                        onUpdate: update => { model.Progress = update.Percentage ?? 0.0; });
                    
                    var converter = mediaProviderService.CreateConverter(parameter);
                    
                    model.Progress = 0.0;
                    model.Status = OutputStatus.Processing;
                    await converter.ExecuteAsync(cancellationToken);
                    if (cancellationToken.IsCancellationRequested)
                    {
                        model.Status = OutputStatus.Failed;
                    }
                    else
                    {
                        model.Progress = 1.0;
                    }
                    
                    // Updates the status
                    outputService.UpdateStatus(model);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed FFmpeg export for playlist {PlaylistId:00000}!", model.Definition.Identifier.Id);
                    model.Status = OutputStatus.Failed;
                }
            }

            Status = OutputQueueStatus.Idle;
        }, cancellationToken);
    }

    /// <inheritdoc />
    public void Stop()
    {
        if (Status != OutputQueueStatus.Running)
        {
            return;
        }
        
        _cancellationTokenSource?.Cancel();
    }
}