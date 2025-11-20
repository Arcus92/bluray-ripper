using DvdLib.Vmg;
using MediaLib.Dvds.Providers;
using MediaLib.Providers;
using Microsoft.Extensions.Logging;

namespace MediaLib.Dvds.Exporter;

public class DvdMediaConverter : FFmpegMediaConverter<DvdMediaProvider>
{
    public DvdMediaConverter(ILogger logger, DvdMediaProvider provider, MediaConverterParameter parameter) : 
        base(logger, provider, parameter)
    {
    }

    /// <inheritdoc />
    protected override long GetSegmentFilesize(ushort segmentId)
    {
        var identifier = VmgIdentifier.FromSegmentId(segmentId);
        var fileInfo = Provider.Dvd.GetVobFileInfo(identifier);
        return fileInfo.Length;
    }
    
    /// <inheritdoc />
    protected override Stream OpenSegmentStream(ushort segmentId)
    {
        var identifier = VmgIdentifier.FromSegmentId(segmentId);
        
        var retries = 0;
        const int maxRetries = 5;
        while (true)
        {
            try
            {
                Logger.LogInformation("Opening segment {identifier}.VOB", identifier);
                return Provider.Dvd.GetVobStream(identifier);
            }
            catch (Exception ex)
            {
                if (retries < maxRetries)
                {
                    retries++;
                    Logger.LogWarning(ex, "Exception while opening segment {identifier}.VOB. Retry {Retry} / {MaxRetry}", identifier, retries, maxRetries);
                }
                else
                {
                    Logger.LogError(ex, "Exception while opening segment {identifier}.VOB!", identifier);
                    throw;
                }
            }
        }
    }

    
}