using System;
using System.IO;
using System.Threading.Tasks;
using BluRayLib.Ripper;
using BluRayLib.Ripper.BluRays;
using BluRayLib.Ripper.BluRays.Export;

namespace BluRayRipper.Services.Interfaces;

public interface IDiskService
{
    /// <summary>
    /// Gets the currently loaded path of the disk.
    /// </summary>
    string DiskPath { get; }
    
    /// <summary>
    /// Gets the currently loaded disk name.
    /// </summary>
    string DiskName { get; }
    
    /// <summary>
    /// Gets the disk info.
    /// </summary>
    DiskInfo Info { get; }
    
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
    TitleData[] GetTitles();
    
    /// <summary>
    /// Creates a track from the current disk.
    /// </summary>
    /// <param name="playlistId">The playlist id.</param>
    /// <returns>The track info.</returns>
    TitleData GetTitle(ushort playlistId);
    
    /// <summary>
    /// Creates a new title exporter.
    /// </summary>
    /// <returns></returns>
    TitleExporter CreateTitleExporter();
    
    /// <summary>
    /// Returns the raw M2TS stream of the given segment.
    /// </summary>
    /// <param name="clipId">The clip id.</param>
    /// <returns>Returns the stream.</returns>
    Stream GetSegmentStream(ushort clipId);
}