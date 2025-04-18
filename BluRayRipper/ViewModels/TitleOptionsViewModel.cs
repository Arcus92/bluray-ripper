using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using BluRayLib.Ripper;
using BluRayRipper.Models;
using BluRayRipper.Models.Output;
using BluRayRipper.Services.Interfaces;
using BluRayRipper.Views;

namespace BluRayRipper.ViewModels;

public class TitleOptionsViewModel : ViewModelBase
{
    private readonly IOutputService _outputService;
    private readonly IDiskService _diskService;
    private readonly OutputSelectorViewModel _outputSelector;
    private readonly TitleTreeViewModel _titleTree;
    
    public TitleOptionsViewModel(IOutputService outputService, IDiskService diskService, 
        OutputSelectorViewModel outputSelector, TitleTreeViewModel titleTree)
    {
        _outputService = outputService;
        _diskService = diskService;
        _outputSelector = outputSelector;
        _titleTree = titleTree;
    }
    
    #region Format settings

    /// <summary>
    /// Gets the list of all output formats.
    /// </summary>
    public VideoFormat[] AllOutputFormats => VideoFormat.All;
    
    /// <inheritdoc cref="VideoFormat"/>
    private VideoFormat _videoFormat = VideoFormat.Mp4;

    /// <summary>
    /// Gets and sets the output format.
    /// </summary>
    public VideoFormat VideoFormat
    {
        get => _videoFormat;
        set => SetProperty(ref _videoFormat, value);
    }

    #endregion Format settings
    
    #region Export
    
    /// <summary>
    /// Convert video to web-friendly format.
    /// </summary>
    private static readonly CodecOptions DefaultCodecOptions = new()
    {
        VideoCodec = "libx264",
        ConstantRateFactor = 16,
        MaxRate = 20000,
        BufferSize = 25000
    };
    
    /// <summary>
    /// Adds the selected title to the output list.
    /// </summary>
    public async Task QueueSelectionAsync()
    {
        var selectedNode = _titleTree.SelectedTitle;
        if (selectedNode is null) return;
        
        var title = selectedNode.Playlist;
        var baseName = $"{_outputSelector.OutputFilename}_{title.Id}";
        var outputInfo = _outputService.BuildOutputInfo(title, _videoFormat, DefaultCodecOptions, baseName);

        await _outputService.AddAsync(outputInfo);
    }

    public async Task DequeueSelectionAsync()
    {
        var selectedNode = _titleTree.SelectedTitle;
        if (selectedNode is null) return;
        
        var title = selectedNode.Playlist;
        var output = _outputService.GetByPlaylist(_diskService.DiskName, title.Id);
        if (output is null) return;
        if (output.Status == OutputStatus.Completed) return; // Do not remove completed outputs!
        await _outputService.RemoveAsync(output);
    }

    public async Task PlayPreviewAsync()
    {
        // TODO: Clean up
        
        var selectedNode = _titleTree.SelectedTitle;
        if (selectedNode is null) return;
        
        var title = selectedNode.Playlist;
        
        // We can only play segments - not playlists. We must place the preview button on segment level.
        // We should also try to show the audio and subtitle selection listed by the playlist put them as parameter for
        // FFplay.
        var segment = title.Segments.First();

        // Pipe-ing the decrypted segment stream into your player. You won't be able to seek properly.
        // You can skip a few seconds ahead, but not backwards. Changing audio or subtitle tracks will force you to 
        // jump ahead to the end of the current playback buffer.
        // Despite all of this, this is a usable preview to determine the content.
        var process = new Process();
        process.StartInfo.FileName = "ffplay"; // mpv is also working
        process.StartInfo.Arguments = "-";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardInput = true;

        process.Start();
        
        await Task.Run(async () =>
        {
            try
            {
                await using var stream = _diskService.GetSegmentStream(segment.Id);
                await stream.CopyToAsync(process.StandardInput.BaseStream);
            }
            catch (IOException)
            {
                // Broken pipe exception is expected when consuming player is closed...
            }
        });
        
        await process.WaitForExitAsync();
    }
    
    #endregion Export
    
    /// <inheritdoc />
    public override Control CreateView()
    {
        return new TitleOptionsView();
    }
}