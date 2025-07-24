using System.Text.Json;
using System.Text.Json.Serialization;
using MediaLib.Models;

namespace MediaLib.Serializer;

/// <summary>
/// A converter that writes the id in the <see cref="MediaIdentifier"/>. This can be a string or a number.
/// </summary>
public class IdentifierIdConverter : JsonConverter<string>
{
    /// <inheritdoc />
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                return reader.GetString();
            case JsonTokenType.Number:
                return reader.GetInt64().ToString();
            default:
                throw new JsonException();
        }
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        // Try number first
        if (long.TryParse(value, out var number))
        {
            writer.WriteNumberValue(number);
            return;
        }
        
        writer.WriteStringValue(value);
    }
}