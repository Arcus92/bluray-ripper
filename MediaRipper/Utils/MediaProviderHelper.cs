using System;
using System.IO;
using System.Threading.Tasks;
using MediaLib.BluRays.Providers;
using MediaLib.Dvds.Providers;
using MediaLib.FileSystem.Providers;
using MediaLib.Providers;

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
        
        if (BluRayMediaProvider.TryCreate(serviceProvider, path, out var bluRayMediaProvider))
            return Task.FromResult<IMediaProvider>(bluRayMediaProvider);
        
        if (DvdMediaProvider.TryCreate(serviceProvider, path, out var dvdMediaProvider))
            return Task.FromResult<IMediaProvider>(dvdMediaProvider);
        
        if (FileSystemMediaProvider.TryCreate(serviceProvider, path, out var fileSystemMediaProvider))
            return Task.FromResult<IMediaProvider>(fileSystemMediaProvider);
        
        throw new ArgumentException($"{path} is not a valid path");
    }
}