using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using BluRayLib.Ripper;
using BluRayLib.Ripper.BluRays;
using BluRayRipper.Models.Nodes;
using BluRayRipper.Services.Interfaces;
using BluRayRipper.Views;

namespace BluRayRipper.ViewModels;

public class TitleTreeViewModel : ViewModelBase
{
    private readonly IDiskService _diskService;
    
    public TitleTreeViewModel(IDiskService diskService)
    {
        _diskService = diskService;
        _diskService.Loaded += OnDiskServiceLoaded;
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

    public bool TryGetSelectedTitle([MaybeNullWhen(false)] out TitleData title)
    {
        if (_selectedNode is TitleNode node)
        {
            title = node.Playlist;
            return true;
        }

        title = null;
        return false;
    }
    
    public bool TryGetSelectedSegment([MaybeNullWhen(false)] out SegmentData segment)
    {
        if (_selectedNode is SegmentNode node)
        {
            segment = node.Segment;
            return true;
        }

        segment = null;
        return false;
    }
    
    private void OnDiskServiceLoaded(object? sender, EventArgs e)
    {
        BuildTrackNodes();
    }

    private void BuildTrackNodes()
    {
        TitleNodes.Clear();
        var playlists = _diskService.GetTitles();

        foreach (var playlist in playlists)
        {
            var isIgnored = playlist.IgnoreFlags != TitleIgnoreFlags.None;
            TitleNodes.Add(new TitleNode(playlist)
            {
                IsIgnored = isIgnored
            });
        }
    }
    
    /// <summary>
    /// The title nodes for the tree-view.
    /// </summary>
    public ObservableCollection<TitleNode> TitleNodes { get; } = [];
    
    /// <inheritdoc />
    public override Control CreateView()
    {
        return new TitleTreeView();
    }
}