using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BluRayLib.Ripper;
using BluRayLib.Ripper.BluRays;
using BluRayLib.Ripper.BluRays.Export;
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
        foreach (var output in outputService.Items)
        {
            if (output.Status == OutputStatus.Completed) continue;
            if (output.DiskName != diskService.DiskName) continue;
            _queue.Add(output);
        }
        
        // Start the thread
        _isRunning = true;
        await Task.Run(async () =>
        {
            var exporter = diskService.CreateTitleExporter();

            foreach (var output in _queue)
            {
                try
                {
                    var options = BuildExportOptionsFormOutputFile(output.File);
                    var outputPath = outputService.OutputPath;
                    
                    output.Status = OutputStatus.Running;
                    await exporter.ExportAsync(outputPath, options,
                        onUpdate: update => { output.Progress = update.Percentage ?? 0.0; });
                    output.Progress = 1.0;
                    output.Status = OutputStatus.Completed;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed FFmpeg export for playlist {PlaylistId:00000}!", output.PlaylistId);
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
    
    /// <inheritdoc />
    public TitleExportOptions BuildExportOptionsFormOutputFile(OutputFile outputFile)
    {
        var title = diskService.GetTitle(outputFile.PlaylistId);
        var format = VideoFormat.FromExtension(outputFile.Extension) ?? VideoFormat.Mkv;
        var options = TitleExportOptions.From(title, outputFile.BaseName);
        options.Extension = format.Extension;
        options.VideoFormat = format.FFmpegFormat;
        options.Codec = outputFile.Codec;
        options.ExportSubtitlesAsSeparateFiles = !format.SupportPgs;
        
        options.NameMap = new TitleNameMap();
        
        // Change the main video filename
        var filename = $"{options.Basename}{options.Extension}";
        options.NameMap.Add(0, filename);
        
        // Change subtitle filenames
        foreach (var stream in outputFile.SubtitleStreams)
        {
            if (stream.Extension is null) continue;
            filename = $"{options.Basename}{stream.Extension}";
            options.NameMap.Add(stream.Id, filename);
        }
        
        return options;
    }
}