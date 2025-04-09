using System;
using System.Threading.Tasks;
using BluRayLib;
using BluRayLib.Ripper;
using BluRayLib.Ripper.Export;
using BluRayLib.Ripper.Info;
using BluRayRipper.Services.Interfaces;

namespace BluRayRipper.Services;

public class DiskService : IDiskService
{
    /// <summary>
    /// The current opened BluRay disk.
    /// </summary>
    private BluRay? _bluRay;
    
    /// <inheritdoc />
    public async Task OpenAsync(string path)
    {
        await CloseAsync();
        _bluRay = new BluRay(path);
        await _bluRay.LoadAsync();
        IsLoaded = true;
        Loaded?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc />
    public Task CloseAsync()
    {
        if (!IsLoaded) return Task.CompletedTask;
        _bluRay = null;
        IsLoaded = false;
        Unloaded?.Invoke(this, EventArgs.Empty);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public bool IsLoaded { get; private set; }
    
    /// <inheritdoc />
    public event EventHandler? Loaded;
    
    /// <inheritdoc />
    public event EventHandler? Unloaded;
    
    /// <inheritdoc />
    public PlaylistInfo[] GetPlaylistInfos()
    {
        if (_bluRay is null) throw new ArgumentException("BluRay is not loaded!");
        return _bluRay.GetPlaylistInfos();
    }

    /// <inheritdoc />
    public PlaylistExporter CreatePlaylistExporter(ushort playlistId)
    {
        if (_bluRay is null) throw new ArgumentException("BluRay is not loaded!");
        return _bluRay.CreatePlaylistExporter(playlistId);
    }
}