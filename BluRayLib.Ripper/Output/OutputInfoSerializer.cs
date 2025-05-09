using System.Text.Json;

namespace BluRayLib.Ripper.Output;

public static class OutputInfoSerializer
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
    };
    
    /// <summary>
    /// Serializes the output info file.
    /// </summary>
    /// <param name="path">The path to the output info file.</param>
    /// <param name="output">The output info file to write.</param>
    public static async Task SerializeAsync(string path, OutputInfo output)
    {
        await using var stream = File.Create(path);
        await JsonSerializer.SerializeAsync(stream, output, SerializerOptions);
    }
    
    /// <summary>
    /// Deserialize the output info file.
    /// </summary>
    /// <param name="path">The path to the output info file.</param>
    /// <returns>Returns the output info file if valid.</returns>
    public static async Task<OutputInfo?> DeserializeAsync(string path)
    {
        await using var stream = File.OpenRead(path);
        return await JsonSerializer.DeserializeAsync<OutputInfo>(stream, SerializerOptions);
    }

    /// <summary>
    /// Reads the output directory and returns all output files.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>Returns a list of all output info files.</returns>
    public static async IAsyncEnumerable<(string, OutputInfo)> DeserializeFromDirectoryAsync(string path)
    {
        foreach (var file in Directory.EnumerateFiles(path, "*.json"))
        {
            var outputInfo = await DeserializeAsync(file);
            if (outputInfo is null) continue;
            
            var filename = Path.GetFileNameWithoutExtension(file);
            
            yield return (filename, outputInfo);
        }
    }
}