using System.Diagnostics.CodeAnalysis;
using System.Text;
using DvdLib;
using DvdLib.Decrypt;
using MediaLib.Dvds.Exporter;
using MediaLib.Models;
using MediaLib.Providers;
using MediaLib.Sources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MediaLib.Dvds.Providers;

public class DvdMediaProvider : IMediaProvider
{
    private readonly ILogger _logger;
    
    /// <summary>
    /// Gets the Dvd disk information.
    /// </summary>
    public Dvd Dvd { get; }

    public DvdMediaProvider(IServiceProvider serviceProvider, string path)
    {
        _logger = serviceProvider.GetRequiredService<ILogger<DvdMediaProvider>>();;
        Dvd = new Dvd(path);
    }
    
    static DvdMediaProvider()
    {
        DvdCss.RegisterLibraryImportResolver();
    }
    
    /// <inheritdoc />
    public async Task<List<IMediaSource>> GetSourcesAsync()
    {
        var list = new List<IMediaSource>();

        // TODO
        
        return list;
    }

    /// <inheritdoc />
    public IMediaConverter CreateConverter(MediaConverterParameter parameter)
    {
        return new DvdMediaConverter(_logger, this, parameter);
    }

    /// <inheritdoc />
    public Stream GetRawStream(IMediaSource source)
    {
        if (!Contains(source.Identifier)) throw new ArgumentException($"The given source isn't contained by this provider.", nameof(source));

        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public bool Contains(MediaIdentifier identifier)
    {
        return identifier.Type == MediaIdentifierType.Dvd && identifier.ContentHash == Dvd.ContentHash;
    }
    
    /// <summary>
    /// Tries to create a media converter for the given directory.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="path">The disk path.</param>
    /// <param name="provider">Returns the created provider.</param>
    /// <returns>Returns if the path is valid and a provider was created.</returns>
    public static bool TryCreate(IServiceProvider serviceProvider, string path, [MaybeNullWhen(false)] out DvdMediaProvider provider)
    {
        if (!Directory.Exists(Path.Combine(path, "VIDEO_TS")))
        {
            provider = null;
            return false;
        }
        
        provider = new DvdMediaProvider(serviceProvider, path);
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