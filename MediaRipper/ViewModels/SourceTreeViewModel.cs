using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Avalonia.Controls;
using MediaLib;
using MediaRipper.Models.Sources;
using MediaRipper.Services.Interfaces;
using MediaRipper.Views;
using Microsoft.Extensions.Logging;

namespace MediaRipper.ViewModels;

public class SourceTreeViewModel : ViewModelBase
{
    private readonly ILogger<SourceTreeViewModel> _logger;
    private readonly IMediaProviderService _mediaProviderService;
    
    public SourceTreeViewModel(ILogger<SourceTreeViewModel> logger, IMediaProviderService mediaProviderService)
    {
        _logger = logger;
        _mediaProviderService = mediaProviderService;
        _mediaProviderService.Changed += OnMediaProviderServiceChanged;
    }

    /// <inheritdoc cref="SelectedItem"/>
    private BaseSourceModel? _selectedItem;

    /// <summary>
    /// Gets and sets the selected title info.
    /// </summary>
    public BaseSourceModel? SelectedItem
    {
        get => _selectedItem;
        set => SetProperty(ref _selectedItem, value);
    }

    public bool TryGetSelectedTitleNode([MaybeNullWhen(false)] out MediaSourceModel media)
    {
        if (_selectedItem is MediaSourceModel node)
        {
            media = node;
            return true;
        }

        media = null;
        return false;
    }
    
    private async void OnMediaProviderServiceChanged(object? sender, EventArgs e)
    {
        try
        {
            await BuildTrackNodesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to build track nodes!");
        }
    }

    private async Task BuildTrackNodesAsync()
    {
        Items.Clear();
        if (!_mediaProviderService.IsLoaded) return;
        
        var sources = await _mediaProviderService.GetSourcesAsync();
        foreach (var source in sources)
        {
            var isIgnored = source.IgnoreFlags != MediaIgnoreFlags.None;
            Items.Add(new MediaSourceModel(source)
            {
                IsIgnored = isIgnored
            });
        }
    }
    
    /// <summary>
    /// The title nodes for the tree-view.
    /// </summary>
    public ObservableCollection<MediaSourceModel> Items { get; } = [];
    
    /// <inheritdoc />
    public override Control CreateView()
    {
        return new SourceTreeView();
    }
}