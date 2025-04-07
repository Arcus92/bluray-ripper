using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BluRayLib;
using BluRayLib.FFmpeg;
using BluRayLib.Ripper;
using BluRayLib.Ripper.Export;
using BluRayLib.Ripper.Info;
using BluRayRipper.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using MakeMkvLib;

namespace BluRayRipper.Controller;

public class BluRayController : ObservableObject
{
    /// <summary>
    /// The current opened BluRay disk.
    /// </summary>
    private BluRay? _current;
    

    /// <summary>
    /// Gets if the BluRay disk is loaded.
    /// </summary>
    public bool IsLoaded
    {
        get => _isLoaded;
        set => SetProperty(ref _isLoaded, value);
    }

    private bool _isLoaded;

    /// <summary>
    /// Opens the given BluRay disk.
    /// </summary>
    /// <param name="path"></param>
    public async Task OpenAsync(string path)
    {
        Close();
        _current = new BluRay(path);
        await _current.LoadAsync();
        IsLoaded = true;
    }

    /// <summary>
    /// Closes the current disk.
    /// </summary>
    public void Close()
    {
        _current = null;
        IsLoaded = false;
    }

    public PlaylistInfo[] GetPlaylistInfos()
    {
        if (_current is null) throw new ArgumentException("BluRay is not loaded!");
        return _current.GetPlaylistInfos();
    }
    
    #region Export

    public async Task DecryptClipAsync(ushort clipId, string output)
    {
        if (_current is null) throw new ArgumentException("BluRay is not loaded!");
        await using var input = _current.GetDecryptM2TsStream(clipId);
        await using var fileStream = File.Create(output);
        await input.CopyToAsync(fileStream);
    }

    public PlaylistExporter CreatePlaylistExporter(ushort playlistId)
    {
        if (_current is null) throw new ArgumentException("BluRay is not loaded!");
        return _current.CreatePlaylistExporter(playlistId);
    }
    
    #endregion Export
}