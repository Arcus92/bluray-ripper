using Microsoft.Extensions.Logging;

namespace MediaRipper.Utils;

/// <summary>
/// Creates an empty logger using in the UI designer.
/// </summary>
public static class EmptyLogger
{
    private static readonly LoggerFactory LoggerFactory = new();
    
    /// <summary>
    /// Creates a new empty logger.
    /// </summary>
    /// <typeparam name="T">The logger target class.</typeparam>
    /// <returns></returns>
    public static ILogger<T> Create<T>()
    {
        return new Logger<T>(LoggerFactory);
    }
}