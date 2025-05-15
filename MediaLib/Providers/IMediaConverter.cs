using MediaLib.Output;
using MediaLib.Sources;

namespace MediaLib.Providers;

/// <summary>
/// An exporter to write a <see cref="IMediaSource"/> to a <see cref="OutputFile"/>.
/// </summary>
public interface IMediaConverter
{
    /// <summary>
    /// Exports the media to the output location provided by the parameter.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    Task ExecuteAsync(CancellationToken cancellationToken = default);
}