using MediaLib.FileSystem.Providers;
using MediaLib.Providers;
using Microsoft.Extensions.Logging;

namespace MediaLib.FileSystem.Exporter;

/// <summary>
/// A media converter implementation for local files.
/// </summary>
public class FileSystemMediaConverter : FFmpegMediaConverter<FileSystemMediaProvider>
{
    public FileSystemMediaConverter(ILogger logger, FileSystemMediaProvider provider, MediaConverterParameter parameter) : base(logger, provider, parameter)
    {
    }

    /// <inheritdoc />
    protected override long GetSegmentFilesize(ushort segmentId)
    {
        var path = Provider.GetMediaPath(Parameter.Definition.Identifier);
        var fileInfo = new FileInfo(path);
        return fileInfo.Length;
    }
    
    /// <inheritdoc />
    protected override Stream OpenSegmentStream(ushort segmentId)
    {
        var path = Provider.GetMediaPath(Parameter.Definition.Identifier);
        
        Logger.LogInformation("Opening segment {SegmentId}", segmentId);
        return File.OpenRead(path);
    }
}