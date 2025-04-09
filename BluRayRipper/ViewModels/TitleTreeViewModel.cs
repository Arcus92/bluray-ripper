using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using BluRayLib.Ripper.Info;
using BluRayRipper.Models;
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

    // Designer default
    public TitleTreeViewModel() : this(new DiskService())
    {
    }
    
    private void OnDiskServiceLoaded(object? sender, EventArgs e)
    {
        BuildTrackNodes();
    }

    private void BuildTrackNodes()
    {
        TrackNodes.Clear();
        var playlists = _diskService.GetPlaylistInfos();

        foreach (var playlist in playlists)
        {
            TrackNodes.Add(new TrackItem(playlist.ToString(), [
                new TrackItem("Segments", playlist.Segments.Select(segment => new TrackItem(segment.ToString(), [
                    new TrackItem("Video streams", segment.VideoStreams.Select(stream => new TrackItem(stream.ToString()))),
                    new TrackItem("Audio streams", segment.AudioStreams.Select(stream => new TrackItem(stream.ToString()))),
                    new TrackItem("Subtitle streams", segment.SubtitleStreams.Select(stream => new TrackItem(stream.ToString()))),
                ]))),
                new TrackItem("Chapters", playlist.Chapters.Select(chapter => new TrackItem(chapter.ToString())))
            ])
            {
                IsChecked = playlist.IgnoreFlags == PlaylistIgnoreFlags.None
            });
        }
    }
    
    /// <summary>
    /// The track nodes.
    /// </summary>
    public ObservableCollection<TrackItem> TrackNodes { get; } = [];
    
    /// <inheritdoc />
    public override Control CreateView()
    {
        return new TitleTreeView();
    }
}