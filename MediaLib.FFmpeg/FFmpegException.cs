namespace MediaLib.FFmpeg;

/// <summary>
/// An exception when FFmpeg returned an invalid exit code.
/// </summary>
public class FFmpegException : Exception
{
    public FFmpegException(int exitCode, string output) : base($"FFmpeg returned {exitCode}!\n{output}")
    {
        ExitCode = exitCode;
        Output = output;
    }
    
    /// <summary>
    /// Gets the exit code of FFmpeg.
    /// </summary>
    public int ExitCode { get; init; }

    /// <summary>
    /// Gets the FFmpeg console output.
    /// </summary>
    public string Output { get; init; }
}