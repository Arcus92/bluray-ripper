using MediaLib.FFmpeg;
using MediaLib.Output;

namespace MediaLib.Providers;

public readonly struct MediaConverterParameter
{
    /// <summary>
    /// Creates a new media converter parameter.
    /// </summary>
    /// <param name="path">The output path.</param>
    /// <param name="definition">The output definition.</param>
    /// <param name="onUpdate">The status update event.</param>
    public MediaConverterParameter(string path, OutputDefinition definition, Action<ConverterUpdate>? onUpdate = null)
    {
        Path = path;
        Definition = definition;
        OnUpdate = onUpdate;
    }

    /// <summary>
    /// Gets the output path for the export destination.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Gets the output definition.
    /// </summary>
    public OutputDefinition Definition { get; }
    
    /// <summary>
    /// Gets the optional update event for the export.
    /// </summary>
    public Action<ConverterUpdate>? OnUpdate { get; }
}