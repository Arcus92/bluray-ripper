using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MediaLib.Models;
using MediaLib.Providers;
using MediaLib.Sources;

namespace MediaRipper.Services.Interfaces;

public interface IMediaProviderService
{ 
    /// <summary>
    /// Opens a media provider at the given path.
    /// </summary>
    /// <param name="path">The disk path.</param>
    Task OpenAsync(string path);
    
    /// <summary>
    /// Closes the current open media provider.
    /// </summary>
    Task CloseAsync();
    
    /// <summary>
    /// Gets if a media provider is opened loaded.
    /// </summary>
    bool IsLoaded { get; }
    
    /// <summary>
    /// Event that is invoked once a media provider was changed.
    /// </summary>
    event EventHandler? Changed;

    /// <summary>
    /// Creates the track list of the current loaded media provider.
    /// </summary>
    /// <returns>The track list.</returns>
    Task<List<IMediaSource>> GetSourcesAsync();
    
    /// <summary>
    /// Creates a new title exporter from the current media provider.
    /// </summary>
    /// <returns></returns>
    IMediaConverter CreateConverter(MediaConverterParameter parameter);
    
    /// <summary>
    /// Returns the raw media stream from the given source.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    Stream GetRawStream(IMediaSource source);
    
    /// <summary>
    /// Gets if this media provider matches the given output source.
    /// </summary>
    /// <param name="identifier">The output source.</param>
    /// <returns>Returns true, if this media output is compatible with this media provider.</returns>
    bool Contains(MediaIdentifier identifier);
}