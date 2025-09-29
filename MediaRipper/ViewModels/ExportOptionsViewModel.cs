using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using MediaLib;
using MediaRipper.Models.Outputs;
using MediaRipper.Services.Interfaces;
using MediaRipper.Views;

namespace MediaRipper.ViewModels;

public class ExportSettingsViewModel : ViewModelBase
{
    private readonly IOutputService _outputService;
    private readonly IMediaProviderService _mediaProviderService;
    private readonly OutputSelectorViewModel _outputSelector;
    private readonly SourceTreeViewModel _sourceTree;
    
    public ExportSettingsViewModel(IOutputService outputService, IMediaProviderService mediaProviderService, 
        OutputSelectorViewModel outputSelector, SourceTreeViewModel sourceTree)
    {
        _outputService = outputService;
        _mediaProviderService = mediaProviderService;
        _outputSelector = outputSelector;
        _sourceTree = sourceTree;
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
        if (!_sourceTree.TryGetSelectedTitleNode(out var titleNode))
            return;
        
        var outputDefinition = titleNode.Source.CreateDefaultOutputDefinition(DefaultCodecOptions, _videoFormat);
        await _outputService.AddAsync(outputDefinition);
    }

    public async Task DequeueSelectionAsync()
    {
        if (!_sourceTree.TryGetSelectedTitleNode(out var title))
            return;
        
        var output = _outputService.GetByIdentifier(title.Source.Identifier);
        if (output is null) return;
        if (output.Status == OutputStatus.Completed) return; // Do not remove completed outputs!
        await _outputService.RemoveAsync(output);
    }

    public async Task PlayPreviewAsync()
    {
        if (!_sourceTree.TryGetSelectedTitleNode(out var titleNode))
        {
            return;
        }

        // Pipe-ing the decrypted segment stream into your player. You won't be able to seek properly.
        // You can skip a few seconds ahead, but not backwards. Changing audio or subtitle tracks will force you to 
        // jump ahead to the end of the current playback buffer.
        // Despite all of this, this is a usable preview to determine the content.
        var process = new Process();
        process.StartInfo.FileName = "mpv"; // mpv is also working
        process.StartInfo.Arguments = "-";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardInput = true;

        process.Start();
        
        await Task.Run(async () =>
        {
            try
            {
                await using var stream = _mediaProviderService.GetRawStream(titleNode.Source);
                await stream.CopyToAsync(process.StandardInput.BaseStream);
            }
            catch (IOException)
            {
                // Broken pipe exception is expected when consuming player is closed...
            }
        });
        
        await process.WaitForExitAsync();
    }

    public async Task SaveSegmentAsync()
    {
        if (!_sourceTree.TryGetSelectedTitleNode(out var titleNode))
        {
            return;
        }
        var identifier = titleNode.Source.Identifier; // TODO: Filename
        var path = Path.Combine(_outputSelector.OutputPath, $"{identifier.DiskName}_{identifier.Id:00000}.m2ts");
        
        await using var stream = _mediaProviderService.GetRawStream(titleNode.Source);
        await using var output = File.Create(path);
        await stream.CopyToAsync(output);
    }
    
    #endregion Export
    
    /// <inheritdoc />
    public override Control CreateView()
    {
        return new ExportSettingsView();
    }
}