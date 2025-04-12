using Microsoft.Extensions.Logging;

namespace BluRayRipper.Utils;

/// <summary>
/// Creates an empty logger using in the UI designer.
/// </summary>
/// <typeparam name="T"></typeparam>
public static class EmptyLogger<T>
{
    private static readonly LoggerFactory LoggerFactory = new();
    
    /// <summary>
    /// Creates a new empty logger.
    /// </summary>
    /// <returns></returns>
    public static ILogger<T> Create()
    {
        return new Logger<T>(LoggerFactory);
    }
}