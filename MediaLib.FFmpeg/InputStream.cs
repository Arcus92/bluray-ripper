using System.IO.Pipes;
using System.Net.Sockets;

namespace MediaLib.FFmpeg;

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

    private const int HResultPipeIsBroken = -2147024664;
    
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
            try
            {
                // Create the pipe
                _pipe = new NamedPipeServerStream(PipeName, PipeDirection.Out, 10, PipeTransmissionMode.Byte,
                    PipeOptions.FirstPipeInstance, 500000, 500000);

                // Then release the semaphore. This ensures the pipe is created and the following methods can consume it.
                semaphore.Release();

                // Opens the underlying stream once a client connects.
                await _pipe.WaitForConnectionAsync();
                await using var stream = _streamFunc();
                _stream = stream;
                await stream.CopyToAsync(_pipe);
                _fixedPosition = stream.Position;
            }
            catch (IOException ex) when (ex.InnerException is SocketException { ErrorCode: 32 } || ex.HResult == HResultPipeIsBroken)
            {
                // Ignore Broken pipe. This happens when the consuming process closes the pipe.
                // That's fine. We are only concerned about reading errors.  
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                throw;
            }
            finally
            {
                if (_pipe is not null)
                {
                    await _pipe.DisposeAsync();
                }
            }
        });

        await semaphore.WaitAsync();
    }
    
    /// <summary>
    /// Waits for the internal stream reader task.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task WaitAsync(CancellationToken cancellationToken)
    {
        if (_task is null) return;
        await _task.WaitAsync(cancellationToken);
    }
    
    /// <inheritdoc />
    public void Dispose()
    {
        _pipe?.Dispose();
        //_task?.Dispose();
    }
}