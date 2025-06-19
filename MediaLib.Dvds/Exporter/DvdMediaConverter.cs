using MediaLib.Dvds.Providers;
using MediaLib.Providers;
using Microsoft.Extensions.Logging;

namespace MediaLib.Dvds.Exporter;

public class DvdMediaConverter : IMediaConverter
{
    private readonly ILogger _logger;
    private readonly DvdMediaProvider _provider;
    private readonly MediaConverterParameter _parameter;
    
    public DvdMediaConverter(ILogger logger, DvdMediaProvider provider, MediaConverterParameter parameter)
    {
        _logger = logger;
        _provider = provider;
        _parameter = parameter;
    }

    public Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}