using System.Diagnostics;
using System.Text;

namespace BluRayLib.Utils;

/// <summary>
/// A helper video converter class using FFmpeg.
/// </summary>
public class VideoConverter
{
    public VideoConverter(string? ffmpegPath = null)
    {
        FfmpegPath = ffmpegPath ?? "ffmpeg";
    }
    
    /// <summary>
    /// Gets the path to the ffmpeg binary.
    /// </summary>
    public string FfmpegPath { get; }

    /// <summary>
    /// Gets and sets the constant rate factor (CRF).
    /// </summary>
    public int ConstantRateFactor { get; set; } = 14;

    /// <summary>
    /// Gets and sets the max bitrate in k.
    /// </summary>
    public int MaxRate { get; set; } = 20000;

    /// <summary>
    /// Gets and sets the buffer size in k.
    /// </summary>
    public int BufferSize { get; set; } = 25000;

    /// <summary>
    /// Converts the given media file.
    /// </summary>
    /// <param name="input">The source file.</param>
    /// <param name="output">The output file.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task ConvertAsync(string input, string output, CancellationToken cancellationToken)
    {
        var builder = new StringBuilder();
        //builder.Append(" -hwaccel vaapi -hwaccel_device /dev/dri/renderD128 -hwaccel_output_format vaapi");
        //builder.Append("-init_hw_device \"vulkan=vk:0\" -hwaccel vulkan -hwaccel_output_format vulkan");
        builder.Append($" -i \"{input}\"");
        builder.Append(" -c:v libx264");
        //builder.Append(" -c:v hevc_vaapi");
        builder.Append(" -map_chapters 0");
        builder.Append(" -map 0:v");
        builder.Append(" -map 0:a");
        builder.Append($" -crf {ConstantRateFactor}");
        builder.Append($" -maxrate {MaxRate}k");
        builder.Append($" -bufsize {BufferSize}k");
        builder.Append(" -f mp4");
        builder.Append($" \"{output}\"");
        
        var process = new Process();
        process.StartInfo = new ProcessStartInfo()
        {
            FileName = FfmpegPath,
            Arguments = builder.ToString(),
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true
        };
        process.OutputDataReceived += (_, args) =>
        {
            Console.WriteLine(args.Data);
        };
        
        process.ErrorDataReceived += (_, args) =>
        {
            Console.Error.WriteLine(args.Data);
        };
        
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        await process.WaitForExitAsync(cancellationToken);

        if (process.ExitCode != 0)
        {
            throw new IOException($"Ffmpeg returned {process.ExitCode}!");
        }
    }
}