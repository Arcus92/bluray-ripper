using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using BluRayLib.FFmpeg;
using BluRayLib.Ripper.Info;
using BluRayRipper.Controller;
using BluRayRipper.Models;
using BluRayRipper.Views;

namespace BluRayRipper.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    /// <summary>
    /// The BluRay controller.
    /// </summary>
    private readonly BluRayController _bluRayController;
    
    public MainWindowViewModel(BluRayController bluRayController)
    {
        _bluRayController = bluRayController;

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
    public MainWindowViewModel() : this(new BluRayController())
    {
    }
    
    
    /// <summary>
    /// The track nodes.
    /// </summary>
    public ObservableCollection<StreamNode> TrackNodes { get; } = [];

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
    
    /// <inheritdoc cref="ProgressValue"/>
    private double _progressValue;

    /// <summary>
    /// Gets and sets the loading progress value.
    /// </summary>
    public double ProgressValue
    {
        get => _progressValue;
        set => SetProperty(ref _progressValue, value);
    }

    public async Task LoadDiskAsync()
    {
        TrackNodes.Clear();
        OutputFilename = Path.GetFileName(DiskPath);
        await _bluRayController.OpenAsync(DiskPath);
        var playlists = _bluRayController.GetPlaylistInfos();

        foreach (var playlist in playlists)
        {
            TrackNodes.Add(new StreamNode(playlist.ToString(), [
                new StreamNode("Segments", playlist.Segments.Select(segment => new StreamNode(segment.ToString(), [
                    new StreamNode("Video streams", segment.VideoStreams.Select(stream => new StreamNode(stream.ToString()))),
                    new StreamNode("Audio streams", segment.AudioStreams.Select(stream => new StreamNode(stream.ToString()))),
                    new StreamNode("Subtitle streams", segment.SubtitleStreams.Select(stream => new StreamNode(stream.ToString()))),
                ]))),
                new StreamNode("Chapters", playlist.Chapters.Select(chapter => new StreamNode(chapter.ToString())))
            ])
            {
                IsChecked = playlist.IgnoreFlags == PlaylistIgnoreFlags.None
            });
        }
    }
    
    public async Task ExportAsync()
    {
        var playlistId = SelectedPlaylistId;
        
        var outputExtension = ".mp4";
        var outputFormat = "mp4";
        var outputPath = Path.Combine(_outputPath, $"{OutputFilename}_{playlistId:00000}{outputExtension}");
        
        var exporter = _bluRayController.CreatePlaylistExporter(playlistId);
        
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
            
            //builder.ConstantRateFactor(14);
            //builder.MaxRate(20000);
            //builder.BufferSize(25000);
        };
        
        await exporter.ExportAsync(outputPath, outputFormat, onUpdate: update =>
        {
            ProgressValue = update.Percentage ?? 0.0;
        });

        ProgressValue = 1.0;
    }
    
    public async Task DecryptAsync()
    {
        ushort clipId = 9;
        await _bluRayController.DecryptClipAsync(clipId, Path.Combine(_outputPath, $"{OutputFilename}_{clipId:00000}.m2ts"));
    }
    
    /// <inheritdoc />
    public override Control CreateView() => new MainWindow();
}