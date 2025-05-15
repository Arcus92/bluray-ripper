using System;
using System.IO;
using System.Threading.Tasks;
using MediaLib.BluRays.Providers;
using MediaLib.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MediaRipper.Utils;

public static class MediaProviderHelper
{
    /// <summary>
    /// Gets the media provider for the given path.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="path">The path to the media source.</param>
    /// <returns></returns>
    public static Task<IMediaProvider> GetFromPathAsync(IServiceProvider serviceProvider, string path)
    {
        path = Path.GetFullPath(path).TrimEnd('/', '\\'); // Sanitize
        
        // TODO: Implement detection
        var logger = serviceProvider.GetRequiredService<ILogger<BluRayMediaProvider>>();
        IMediaProvider provider = new BluRayMediaProvider(logger, path);

        return Task.FromResult(provider);
    }
}