using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using BluRayLib.FFmpeg;
using BluRayRipper.Models;
using BluRayRipper.Models.Queue;
using BluRayRipper.Services;
using BluRayRipper.Services.Interfaces;
using BluRayRipper.Views;

namespace BluRayRipper.ViewModels;

public class DiskSelectorViewModel : ViewModelBase
{
    private readonly IDiskService _diskService;
    private readonly IQueueService _queueService;

    public DiskSelectorViewModel(IDiskService diskService, IQueueService queueService)
    {
        _diskService = diskService;
        _queueService = queueService;
        
        // Command line shortcut
        var args = Environment.GetCommandLineArgs();
        if (args.Length > 1)
        {
            _diskPath = args[1];
        }
        if (args.Length > 2)
        {
            _outputPath = args[2];
        }
    }
    
    // Designer default
    public DiskSelectorViewModel() : this(new DiskService(), new QueueService())
    {
    }
    
    /// <inheritdoc cref="DiskPath"/>
    private string _diskPath = "";

    /// <summary>
    /// Gets and sets the disk input path.
    /// </summary>
    public string DiskPath
    {
        get => _diskPath;
        set => SetProperty(ref _diskPath, value);
    }
    
    /// <inheritdoc cref="DiskPath"/>
    private string _outputPath = "";

    /// <summary>
    /// Gets and sets the output path.
    /// </summary>
    public string OutputPath
    {
        get => _outputPath;
        set => SetProperty(ref _outputPath, value);
    }
    
    /// <inheritdoc cref="OutputFilename"/>
    private string _outputFilename = "";

    /// <summary>
    /// Gets and sets the output filename.
    /// </summary>
    public string OutputFilename
    {
        get => _outputFilename;
        set => SetProperty(ref _outputFilename, value);
    }

    /// <summary>
    /// Gets the list of all output formats.
    /// </summary>
    public OutputFormat[] AllOutputFormats => OutputFormat.All;
    
    /// <inheritdoc cref="OutputFormat"/>
    private OutputFormat _outputFormat = OutputFormat.Mp4;

    /// <summary>
    /// Gets and sets the output format.
    /// </summary>
    public OutputFormat OutputFormat
    {
        get => _outputFormat;
        set => SetProperty(ref _outputFormat, value);
    }

    /// <inheritdoc cref="SelectedPlaylistId"/>
    private ushort _selectedPlaylistId;

    /// <summary>
    /// Gets and sets the selected playlist id.
    /// </summary>
    public ushort SelectedPlaylistId
    {
        get => _selectedPlaylistId;
        set => SetProperty(ref _selectedPlaylistId, value);
    }
    
    public async Task LoadDiskAsync()
    {
        var diskPath = Path.GetFullPath(DiskPath).TrimEnd('/', '\\'); // Sanitize
        OutputFilename = Path.GetFileName(diskPath);
        await _diskService.OpenAsync(diskPath);
    }
    
    public async Task QueueExportAsync()
    {
        var playlistId = SelectedPlaylistId;
        var outputFormat = _outputFormat;
        var outputFilename = $"{OutputFilename}_{playlistId:00000}{outputFormat.FileExtension}";
        var outputPath = Path.Combine(_outputPath, outputFilename);
            
        var exporter = _diskService.CreatePlaylistExporter(playlistId);
        exporter.ExportSubtitlesAsSeparateFiles = true;
        
        // Convert video to web-friendly format.
        exporter.VideoCommandBuilder = builder =>
        {
            builder.Codec(StreamType.Video, "libx264");
            builder.Codec(StreamType.Audio, "copy");
            builder.Codec(StreamType.Subtitle, "copy");

            builder.ConstantRateFactor(16);
            builder.MaxRate(20000);
            builder.BufferSize(25000);
        };
        
        _queueService.QueueTask(new QueuedTask($"Export title [0x{playlistId:x4}] ({playlistId:00000}): {outputFilename}", async task =>
        {
            await exporter.ExportAsync(outputPath, outputFormat.FFmpegFormat, onUpdate: update =>
            {
                task.Progress = update.Percentage ?? 0.0;
            });

            task.Progress = 1.0;
        }));
    }
    
    /// <inheritdoc />
    public override Control CreateView()
    {
        return new DiskSelectorView();
    }
}