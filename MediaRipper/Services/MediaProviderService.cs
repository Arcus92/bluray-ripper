using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MediaLib.Models;
using MediaLib.Providers;
using MediaLib.Sources;
using MediaRipper.Services.Interfaces;
using MediaRipper.Utils;
using Microsoft.Extensions.Logging;

namespace MediaRipper.Services;

/// <summary>
/// Handles disk loading and collects exportable titles from the disk. Currently, only BluRay is supported.
/// </summary>
public class MediaProviderService : IMediaProviderService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MediaProviderService> _logger;
    
    public MediaProviderService(IServiceProvider serviceProvider, ILogger<MediaProviderService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    
    /// <summary>
    /// The current loaded media provider.
    /// </summary>
    private IMediaProvider? _provider;
    
    /// <inheritdoc />
    public async Task OpenAsync(string path)
    {
        await CloseAsync();
        
        _logger.LogInformation("Opening directory: {Path}", path);
        
        try
        {
            _provider = await MediaProviderHelper.GetFromPathAsync(_serviceProvider, path);

            IsLoaded = true;
            
            _logger.LogInformation("Directory '{Path}' opened by provider {Provider}", path, _provider.GetType().Name);
            Changed?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to load directory: {Path}", path);
            _provider = null;
        }
    }

    /// <inheritdoc />
    public Task CloseAsync()
    {
        if (!IsLoaded) return Task.CompletedTask;
        if (_provider is null) return Task.CompletedTask;
        
        _logger.LogInformation("Closing provider: {Provider}", _provider.GetType().Name);
        
        _provider.Dispose();
        _provider = null;
        IsLoaded = false;
        Changed?.Invoke(this, EventArgs.Empty);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public bool IsLoaded { get; private set; }
    
    /// <inheritdoc />
    public event EventHandler? Changed;
    
    /// <inheritdoc />
    public Task<List<IMediaSource>> GetSourcesAsync()
    {
        if (_provider is null) throw new ArgumentException("No media provider has been loaded!", nameof(_provider));
        return _provider.GetSourcesAsync();
    }
    
    /// <inheritdoc />
    public IMediaConverter CreateConverter(MediaConverterParameter parameter)
    {
        if (_provider is null) throw new ArgumentException("No media provider has been loaded!", nameof(_provider));
        return _provider.CreateConverter(parameter);
    }

    /// <inheritdoc />
    public Stream GetRawStream(IMediaSource source)
    {
        if (_provider is null) throw new ArgumentException("No media provider has been loaded!", nameof(_provider));
        return _provider.GetRawStream(source);
    }

    /// <inheritdoc />
    public bool Contains(MediaIdentifier identifier)
    {
        if (_provider is null) return false;
        return _provider.Contains(identifier);
    }
}