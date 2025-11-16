using System.Text.Json.Serialization;
using MediaLib.Output;

namespace MediaLib.Serializer;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(OutputDefinition))]
public partial class ModelContext : JsonSerializerContext
{
}