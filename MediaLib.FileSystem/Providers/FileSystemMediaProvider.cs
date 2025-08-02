using System.Diagnostics.CodeAnalysis;
using MediaLib.FFmpeg;
using MediaLib.FileSystem.Exporter;
using MediaLib.FileSystem.Sources;
using MediaLib.Models;
using MediaLib.Providers;
using MediaLib.Sources;
using MediaLib.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MediaLib.FileSystem.Providers;

/// <summary>
/// A media provider for local files in a given directory.
/// </summary>
public class FileSystemMediaProvider : IMediaProvider
{
    private readonly ILogger _logger;

    private readonly string _path;
    private readonly string _contentHash;
    private readonly string _diskName;
    public FileSystemMediaProvider(IServiceProvider serviceProvider, string path)
    {
        _logger = serviceProvider.GetRequiredService<ILogger<FileSystemMediaProvider>>();;
        _path = path;
        _contentHash = ContentHash.CalculateHash(path);
        _diskName = Path.GetFileName(_diskName) ?? "";
    }
    
    /// <inheritdoc />
    public async Task<List<IMediaSource>> GetSourcesAsync()
    {
        var list = new List<IMediaSource>();

        foreach (var file in Directory.EnumerateFiles(_path, "*.mkv"))
        {
            var source = await TryGetSourceAsync(file);
            if (source is null) continue;
            list.Add(source);
        }
        
        return list;
    }

    /// <inheritdoc />
    public IMediaConverter CreateConverter(MediaConverterParameter parameter)
    {
        return new FileSystemMediaConverter(_logger, this, parameter);
    }

    /// <inheritdoc />
    public Stream GetRawStream(IMediaSource source)
    {
        if (!Contains(source.Identifier)) throw new ArgumentException($"The given source isn't contained by this provider.", nameof(source));
        
        return File.OpenRead(GetMediaPath(source.Identifier));
    }

    /// <inheritdoc />
    public bool Contains(MediaIdentifier identifier)
    {
        return identifier.Type == MediaIdentifierType.File && identifier.ContentHash == _contentHash &&
               File.Exists(GetMediaPath(identifier));
    }

    /// <summary>
    /// Returns the media source for the given playlist id.
    /// </summary>
    /// <param name="path">The path to the source file.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Returns the source object.</returns>
    public async Task<FileSystemMediaSource?> TryGetSourceAsync(string path, CancellationToken cancellationToken = default)
    {
        var identifier = new MediaIdentifier()
        {
            Type = MediaIdentifierType.File,
            ContentHash = _contentHash,
            DiskName = _diskName,
            Id = Path.GetFileName(path),
            SegmentIds = [0],
        };
        
        try
        {
            var ffmpeg = new Engine();
            var metadata = await ffmpeg.GetMetadataAsync(path, cancellationToken);

            return new FileSystemMediaSource(path, identifier, metadata);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Couldn't fetch metadata with FFmpeg: {Exception}", ex);
            return null;
        }
    }

    /// <summary>
    /// Returns the fill disk path of the media identifier.
    /// </summary>
    /// <param name="identifier">The media identifier.</param>
    /// <returns>Returns the full path.</returns>
    public string GetMediaPath(MediaIdentifier identifier)
    {
        return Path.Combine(_path, identifier.Id);
    }
    
    /// <summary>
    /// Tries to create a media converter for the given directory.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="path">The disk path.</param>
    /// <param name="provider">Returns the created provider.</param>
    /// <returns>Returns if the path is valid and a provider was created.</returns>
    public static bool TryCreate(IServiceProvider serviceProvider, string path, [MaybeNullWhen(false)] out FileSystemMediaProvider provider)
    {
        if (!Directory.Exists(path))
        {
            provider = null;
            return false;
        }
        
        provider = new FileSystemMediaProvider(serviceProvider, path);
        return true;
    }
    
    #region Dispose
    
    /// <inheritdoc />
    public void Dispose()
    {
        // Nothing to do
    }
    
    #endregion Dispose
}