using System.IO.Pipes;

namespace BluRayLib.FFmpeg;

/// <summary>
/// An pipe input stream for FFmpeg.
/// </summary>
public class InputStream : IDisposable
{
    public InputStream(string pipeName, Func<Stream> streamFunc)
    {
        PipeName = pipeName;
        _streamFunc = streamFunc;
    }
    
    public InputStream(string pipeName, Stream stream) : this(pipeName, () => stream)
    {
    }

    /// <summary>
    /// The name of the pipe.
    /// </summary>
    public string PipeName { get; }
    
    /// <summary>
    /// The callback to create the stream.
    /// </summary>
    private readonly Func<Stream> _streamFunc;

    /// <summary>
    /// The current task.
    /// </summary>
    private Task? _task;
    
    /// <summary>
    /// The current pipe.
    /// </summary>
    private NamedPipeServerStream? _pipe;

    /// <summary>
    /// Gets the pipe path.
    /// </summary>
    /// <returns></returns>
    public string GetPath()
    {
        switch (Environment.OSVersion.Platform)
        {
            case PlatformID.Win32NT:
                return $@"\\.\pipe\{PipeName}";
            case PlatformID.Unix:
            case PlatformID.MacOSX: // Not confirmed on MacOS
                return $"unix:{Path.Combine(Path.GetTempPath(), $"CoreFxPipe_{PipeName}")}";
            default:
                throw new PlatformNotSupportedException();
        }
    }
    
    /// <summary>
    /// Opens the input stream pipe.
    /// </summary>
    public void Open()
    {
        _task = Task.Run(async () =>
        {
            _pipe = new NamedPipeServerStream(PipeName, PipeDirection.Out, 10, PipeTransmissionMode.Byte, PipeOptions.FirstPipeInstance, 500000, 500000);
            await _pipe.WaitForConnectionAsync();
            await using var stream = _streamFunc();
            await stream.CopyToAsync(_pipe);
            await _pipe.DisposeAsync();
        });
    }
    
    /// <inheritdoc />
    public void Dispose()
    {
        _pipe?.Dispose();
        //_task?.Dispose();
    }
}