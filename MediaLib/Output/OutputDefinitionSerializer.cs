using System.Text.Json;
using MediaLib.Serializer;

namespace MediaLib.Output;

public static class OutputDefinitionSerializer
{
    /// <summary>
    /// Serializes the output definition file.
    /// </summary>
    /// <param name="path">The path to the output definition file.</param>
    /// <param name="definition">The output definition file to write.</param>
    public static async Task SerializeAsync(string path, OutputDefinition definition)
    {
        await using var stream = File.Create(path);
        await JsonSerializer.SerializeAsync(stream, definition, ModelContext.Default.OutputDefinition);
    }
    
    /// <summary>
    /// Deserialize the output definition file.
    /// </summary>
    /// <param name="path">The path to the output definition file.</param>
    /// <returns>Returns the output definition file if valid.</returns>
    public static async Task<OutputDefinition?> DeserializeAsync(string path)
    {
        await using var stream = File.OpenRead(path);
        return await JsonSerializer.DeserializeAsync(stream, ModelContext.Default.OutputDefinition);
    }

    /// <summary>
    /// Reads the output directory and returns all output files.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>Returns a list of all output definition files.</returns>
    public static async IAsyncEnumerable<(string, OutputDefinition)> DeserializeFromDirectoryAsync(string path)
    {
        foreach (var file in Directory.EnumerateFiles(path, "*.json").Order())
        {
            var definition = await DeserializeAsync(file);
            if (definition is null) continue;
            
            var filename = Path.GetFileNameWithoutExtension(file);
            
            yield return (filename, definition);
        }
    }
}