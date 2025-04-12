using System;
using System.IO;
using System.Threading.Tasks;
using BluRayLib;
using BluRayLib.Decrypt;
using BluRayLib.Ripper;
using BluRayLib.Ripper.BluRays;
using BluRayLib.Ripper.BluRays.Export;
using BluRayRipper.Services.Interfaces;
using BluRayRipper.Utils;
using Microsoft.Extensions.Logging;

namespace BluRayRipper.Services;

/// <summary>
/// Handles disk loading and collects exportable titles from the disk. Currently, only BluRay is supported.
/// </summary>
public class DiskService : IDiskService
{
    private readonly ILogger<DiskService> _logger;
    
    public DiskService(ILogger<DiskService> logger)
    {
        _logger = logger;
        
        // Use MakeMkv as decryption handler. I might add native AACS with key-config file later.
        MakeMkv.RegisterAsDecryptionHandler();
        MakeMkv.RegisterLibraryImportResolver();
    }

    public DiskService() : this(EmptyLogger<DiskService>.Create())
    {
    }
    
    /// <summary>
    /// The current opened BluRay disk.
    /// </summary>
    private BluRay? _bluRay;
    
    /// <inheritdoc />
    public async Task OpenAsync(string path)
    {
        await CloseAsync();
        
        _logger.LogInformation("Opening disk: {DiskPath}", path);
        
        _bluRay = new BluRay(path);

        try
        {
            await _bluRay.LoadAsync();
            IsLoaded = true;
        
            _logger.LogInformation("Disk loaded: {DiskPath}", path);
            Loaded?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to load disk: {DiskPath}", path);
            _bluRay = null;
        }
    }

    /// <inheritdoc />
    public Task CloseAsync()
    {
        if (!IsLoaded) return Task.CompletedTask;
        
        _logger.LogInformation("Closing disk: {DiskPath}", _bluRay?.DiskPath);
        
        _bluRay = null;
        IsLoaded = false;
        Unloaded?.Invoke(this, EventArgs.Empty);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public string DiskPath => _bluRay?.DiskPath ?? string.Empty;

    /// <inheritdoc />
    public bool IsLoaded { get; private set; }
    
    /// <inheritdoc />
    public event EventHandler? Loaded;
    
    /// <inheritdoc />
    public event EventHandler? Unloaded;
    
    /// <inheritdoc />
    public TitleData[] GetTitles()
    {
        if (_bluRay is null) throw new ArgumentException("BluRay is not loaded!");
        return _bluRay.GetTitles();
    }

    /// <inheritdoc />
    public TitleData GetTitle(ushort playlistId)
    {
        if (_bluRay is null) throw new ArgumentException("BluRay is not loaded!");
        return _bluRay.GetTitle(playlistId);
    }

    /// <inheritdoc />
    public TitleExporter CreateTitleExporter()
    {
        if (_bluRay is null) throw new ArgumentException("BluRay is not loaded!");
        return new TitleExporter(_bluRay);
    }
    
    /// <inheritdoc />
    public Stream GetSegmentStream(ushort clipId)
    {
        if (_bluRay is null) throw new ArgumentException("BluRay is not loaded!");
        return _bluRay.GetM2TsStream(clipId);
    }
}