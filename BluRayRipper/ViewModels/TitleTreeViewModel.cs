using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using BluRayLib.Ripper;
using BluRayRipper.Models.Nodes;
using BluRayRipper.Services;
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

    /// <inheritdoc cref="SelectedTitle"/>
    private TitleNode? _selectedTitle;

    /// <summary>
    /// Gets and sets the selected title info.
    /// </summary>
    public TitleNode? SelectedTitle
    {
        get => _selectedTitle;
        set => SetProperty(ref _selectedTitle, value);
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