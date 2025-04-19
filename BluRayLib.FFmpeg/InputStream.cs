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

    private long? _fixedPosition;
    
    /// <summary>
    /// Gets the current file position.
    /// </summary>
    public long Position => _fixedPosition ?? _stream?.Position ?? 0;
    
    /// <summary>
    /// The callback to create the stream.
    /// </summary>
    private readonly Func<Stream> _streamFunc;
    
    /// <summary>
    /// The current stream.
    /// </summary>
    private Stream? _stream;

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
    public async Task StartAsync()
    {
        var semaphore = new SemaphoreSlim(0, 1);
        
        // Stream read thread
        _task = Task.Run(async () =>
        {
            // Create the pipe
            _pipe = new NamedPipeServerStream(PipeName, PipeDirection.Out, 10, PipeTransmissionMode.Byte, PipeOptions.FirstPipeInstance, 500000, 500000);
            
            // Then release the semaphore. This ensures the pipe is created and the following methods can consume it.
            semaphore.Release();
            
            // Opens the underlying stream once a client connects.
            await _pipe.WaitForConnectionAsync();
            await using var stream = _streamFunc();
            _stream = stream;
            await stream.CopyToAsync(_pipe);
            await _pipe.DisposeAsync();
            _fixedPosition = stream.Position;
        });

        await semaphore.WaitAsync();
    }
    
    /// <inheritdoc />
    public void Dispose()
    {
        _pipe?.Dispose();
        //_task?.Dispose();
    }
}