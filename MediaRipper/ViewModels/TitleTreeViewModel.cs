using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Avalonia.Controls;
using MediaLib;
using MediaRipper.Models.Nodes;
using MediaRipper.Services.Interfaces;
using MediaRipper.Views;

namespace MediaRipper.ViewModels;

public class TitleTreeViewModel : ViewModelBase
{
    private readonly IMediaProviderService _mediaProviderService;
    
    public TitleTreeViewModel(IMediaProviderService mediaProviderService)
    {
        _mediaProviderService = mediaProviderService;
        _mediaProviderService.Changed += OnMediaProviderServiceChanged;
    }

    /// <inheritdoc cref="SelectedNode"/>
    private BaseNode? _selectedNode;

    /// <summary>
    /// Gets and sets the selected title info.
    /// </summary>
    public BaseNode? SelectedNode
    {
        get => _selectedNode;
        set => SetProperty(ref _selectedNode, value);
    }

    public bool TryGetSelectedTitleNode([MaybeNullWhen(false)] out MediaNode media)
    {
        if (_selectedNode is MediaNode node)
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
            throw; // TODO handle exception
        }
    }

    private async Task BuildTrackNodesAsync()
    {
        TitleNodes.Clear();
        if (!_mediaProviderService.IsLoaded) return;
        
        var sources = await _mediaProviderService.GetSourcesAsync();
        foreach (var source in sources)
        {
            var isIgnored = source.Info.IgnoreFlags != MediaIgnoreFlags.None;
            TitleNodes.Add(new MediaNode(source)
            {
                IsIgnored = isIgnored
            });
        }
    }
    
    /// <summary>
    /// The title nodes for the tree-view.
    /// </summary>
    public ObservableCollection<MediaNode> TitleNodes { get; } = [];
    
    /// <inheritdoc />
    public override Control CreateView()
    {
        return new TitleTreeView();
    }
}