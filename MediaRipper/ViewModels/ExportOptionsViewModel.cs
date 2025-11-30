using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using MediaLib;
using MediaLib.Formats;
using MediaRipper.Models.Outputs;
using MediaRipper.Services.Interfaces;
using MediaRipper.Views;

namespace MediaRipper.ViewModels;

public class ExportSettingsViewModel : ViewModelBase
{
    private readonly IOutputService _outputService;
    private readonly IMediaProviderService _mediaProviderService;
    private readonly ISettingService _settingService;
    private readonly OutputSelectorViewModel _outputSelector;
    private readonly SourceTreeViewModel _sourceTree;
    private readonly MediaLookupViewModel _mediaLookup;
    
    public ExportSettingsViewModel(IOutputService outputService, IMediaProviderService mediaProviderService, 
        ISettingService settingService, OutputSelectorViewModel outputSelector, SourceTreeViewModel sourceTree, 
        MediaLookupViewModel mediaLookup)
    {
        _outputService = outputService;
        _mediaProviderService = mediaProviderService;
        _settingService = settingService;
        _outputSelector = outputSelector;
        _sourceTree = sourceTree;
        _mediaLookup = mediaLookup;
        
        _sourceTree.PropertyChanged += OnSourceTreePropertyChanged;
    }

    #region Selection
    
    /// <inheritdoc cref="CanQueueSelection" />
    private bool _canQueueSelection;

    /// <summary>
    /// Gets if the selected item can be queued.
    /// </summary>
    public bool CanQueueSelection
    {
        get => _canQueueSelection;
        set => SetProperty(ref _canQueueSelection, value);
    }
    
    /// <inheritdoc cref="CanDequeueSelection" />
    private bool _canDequeueSelection;

    /// <summary>
    /// Gets if the selected item can be dequeued.
    /// </summary>
    public bool CanDequeueSelection
    {
        get => _canDequeueSelection;
        set => SetProperty(ref _canDequeueSelection, value);
    }
    
    /// <inheritdoc cref="CanPlaySelection" />
    private bool _canPlaySelection;

    /// <summary>
    /// Gets if the selected item can be played.
    /// </summary>
    public bool CanPlaySelection
    {
        get => _canPlaySelection;
        set => SetProperty(ref _canPlaySelection, value);
    }
    
    /// <inheritdoc cref="CanSaveSelection" />
    private bool _canSaveSelection;

    /// <summary>
    /// Gets if the selected item can be saved.
    /// </summary>
    public bool CanSaveSelection
    {
        get => _canSaveSelection;
        set => SetProperty(ref _canSaveSelection, value);
    }
    
    private void OnSourceTreePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(SourceTreeViewModel.SelectedItem):
                UpdateSelection();
                break;
        }
    }

    private void UpdateSelection()
    {
        var canPlayPreview = !string.IsNullOrEmpty(_settingService.Data.FFplayPath);
        
        if (_sourceTree.TryGetSelectedTitleNode(out var titleNode))
        {
            var output = _outputService.GetByIdentifier(titleNode.Source.Identifier);
            CanQueueSelection = output is null;
            CanDequeueSelection = output is not null && output.Status == OutputStatus.Queued;
            CanPlaySelection = canPlayPreview;
            CanSaveSelection = true;
        }
        else
        {
            CanQueueSelection = false;
            CanDequeueSelection = false;
            CanPlaySelection = false;
            CanSaveSelection = false;
        }
    }
    
    #endregion Selection

    #region Format settings

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
    /// Gets the list of all output formats.
    /// </summary>
    public MediaFormat[] AllOutputFormats => ContainerFormats.All;
    
    /// <inheritdoc cref="OutputFormat"/>
    private MediaFormat _outputFormat = ContainerFormats.Mp4;

    /// <summary>
    /// Gets and sets the output format.
    /// </summary>
    public MediaFormat OutputFormat
    {
        get => _outputFormat;
        set => SetProperty(ref _outputFormat, value);
    }

    #endregion Format settings
    
    #region Commands
    
    /// <summary>
    /// Adds the selected title to the output list.
    /// </summary>
    public async Task QueueSelectionAsync()
    {
        if (!_sourceTree.TryGetSelectedTitleNode(out var titleNode))
            return;
        
        var outputDefinition = titleNode.Source.CreateDefaultOutputDefinition(DefaultCodecOptions, _outputFormat);

        if (_mediaLookup.TryGetMediaInfo(out var mediaInfo))
        {
            var basename = mediaInfo.GetBasename();

            foreach (var file in outputDefinition.Files)
            {
                file.Filename = file.Filename[outputDefinition.MediaInfo.Name.Length..].Insert(0, basename);
            }
            
            outputDefinition.MediaInfo = mediaInfo;
            _mediaLookup.IncreaseEpisodeNumber();
        }
        
        await _outputService.AddAsync(outputDefinition);
        UpdateSelection();
    }

    public async Task DequeueSelectionAsync()
    {
        if (!_sourceTree.TryGetSelectedTitleNode(out var title))
            return;
        
        var output = _outputService.GetByIdentifier(title.Source.Identifier);
        if (output is null) return;
        if (output.Status == OutputStatus.Completed) return; // Do not remove completed outputs!
        await _outputService.RemoveAsync(output);
        UpdateSelection();
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
        process.StartInfo.FileName = _settingService.Data.FFplayPath;
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
    
    #endregion Commands
    
    /// <inheritdoc />
    public override Control CreateView()
    {
        return new ExportSettingsView();
    }
}