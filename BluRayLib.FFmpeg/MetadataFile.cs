using System.Text;

namespace BluRayLib.FFmpeg;

/// <summary>
/// This class can write FFmpeg metadata files that can be used as input for FFmpeg.
/// </summary>
public class MetadataFile
{
    /// <summary>
    /// Gets and sets the chapters.
    /// </summary>
    public ChapterMetadata[]? Chapters { get; set; }

    /// <summary>
    /// Writes the metadata file to the given path.
    /// </summary>
    /// <param name="filename">The path to the metadata file.</param>
    public async Task Save(string filename)
    {
        await using var stream = new FileStream(filename, FileMode.Create);
        await Save(stream);
    }
    
    /// <summary>
    /// Writes the metadata file to the given stream.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    public async Task Save(Stream stream)
    {
        await using var writer = new StreamWriter(stream, new UTF8Encoding(false));

        await writer.WriteLineAsync(";FFMETADATA1");

        if (Chapters is not null)
        {
            foreach (var chapter in Chapters)
            {
                await writer.WriteLineAsync("[CHAPTER]");
                await writer.WriteLineAsync("TIMEBASE=1/1000");
                await writer.WriteLineAsync($"START={chapter.Start.TotalMilliseconds:#}");
                await writer.WriteLineAsync($"END={chapter.End.TotalMilliseconds:#}");
                await writer.WriteLineAsync($"TITLE={chapter.Title}");
            }
        }
    }
}