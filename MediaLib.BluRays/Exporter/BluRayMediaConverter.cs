using MediaLib.BluRays.Providers;
using MediaLib.FFmpeg;
using MediaLib.Providers;
using Microsoft.Extensions.Logging;

namespace MediaLib.BluRays.Exporter;

public class BluRayMediaConverter : FFmpegMediaConverter<BluRayMediaProvider>
{
    public BluRayMediaConverter(ILogger logger, BluRayMediaProvider provider, MediaConverterParameter parameter) : base(logger, provider, parameter)
    {
    }

    /// <inheritdoc />
    protected override ulong GetStreamIndex(StreamMetadata stream) => stream.Pid;

    /// <inheritdoc />
    protected override long GetSegmentFilesize(ushort segmentId)
    {
        var fileInfo = Provider.BluRay.GetM2TsFileInfo(segmentId);
        return fileInfo.Length;
    }
    
    /// <inheritdoc />
    protected override Stream OpenSegmentStream(ushort segmentId)
    {
        var retries = 0;
        const int maxRetries = 5;
        while (true)
        {
            try
            {
                Logger.LogInformation("Opening segment {SegmentId:00000}.m2ts", segmentId);
                return Provider.BluRay.GetM2TsStream(segmentId);
            }
            catch (Exception ex)
            {
                if (retries < maxRetries)
                {
                    retries++;
                    Logger.LogWarning(ex, "Exception while opening segment {SegmentId:00000}.m2ts. Retry {Retry} / {MaxRetry}", segmentId, retries, maxRetries);
                }
                else
                {
                    Logger.LogError(ex, "Exception while opening segment {SegmentId:00000}.m2ts!", segmentId);
                    throw;
                }
            }
        }
    }
}