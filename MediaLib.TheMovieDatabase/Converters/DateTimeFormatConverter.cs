using System.Text.Json;
using System.Text.Json.Serialization;

namespace MediaLib.TheMovieDatabase.Converters;

/// <summary>
/// A JSON converter to read a specific date-time format. 
/// </summary>
public class DateTimeFormatConverter : JsonConverter<DateTime>
{
    /// <summary>
    /// Gets and sets the date format.
    /// </summary>
    public string DateFormat { get; init; } = "yyyy-MM-dd";
    
    /// <inheritdoc />
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var text = reader.GetString();
        if (string.IsNullOrEmpty(text))
        {
            return default;
        }
        
        return DateTime.ParseExact(text, DateFormat, null);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        if (value == default)
        {
            writer.WriteStringValue("");
            return;
        }
        
        writer.WriteStringValue(value.ToString(DateFormat));
    }
}