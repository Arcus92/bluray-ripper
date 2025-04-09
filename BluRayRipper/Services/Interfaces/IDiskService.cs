using System;
using System.IO;
using System.Threading.Tasks;
using BluRayLib.Ripper.Export;
using BluRayLib.Ripper.Info;

namespace BluRayRipper.Services.Interfaces;

public interface IDiskService
{
    /// <summary>
    /// Opens the disk at the given path.
    /// </summary>
    /// <param name="path">The disk path.</param>
    Task OpenAsync(string path);
    
    /// <summary>
    /// Closes the current open BluRay disk.
    /// </summary>
    Task CloseAsync();
    
    /// <summary>
    /// Gets if a BluRay disk is loaded.
    /// </summary>
    bool IsLoaded { get; }
    
    /// <summary>
    /// Event that is invoked once a disk was loaded.
    /// </summary>
    event EventHandler? Loaded;
    
    /// <summary>
    /// Event that is invoked once the current disk was unloaded.
    /// </summary>
    event EventHandler? Unloaded;

    /// <summary>
    /// Creates the track list of the current disk.
    /// </summary>
    /// <returns>The track list.</returns>
    PlaylistInfo[] GetPlaylistInfos();
    
    /// <summary>
    /// Creates a track from the current disk.
    /// </summary>
    /// <param name="playlistId">The playlist id.</param>
    /// <returns>The track info.</returns>
    PlaylistInfo GetPlaylistInfo(ushort playlistId);
    
    /// <summary>
    /// Returns the raw M2TS stream of the given segment.
    /// </summary>
    /// <param name="clipId">The clip id.</param>
    /// <returns>Returns the stream.</returns>
    Stream GetSegmentStream(ushort clipId);
    
    /// <summary>
    /// Creates a new exporter class for the given playlist id.
    /// </summary>
    /// <param name="playlistId">The playlist id.</param>
    /// <returns>Returns the new exporter.</returns>
    PlaylistExporter CreatePlaylistExporter(ushort playlistId);
}