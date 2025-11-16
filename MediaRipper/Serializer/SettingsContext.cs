using System.Text.Json.Serialization;
using MediaRipper.Models.Settings;

namespace MediaRipper.Serializer;

[JsonSerializable(typeof(SettingsData))]
public partial class SettingsContext : JsonSerializerContext
{
}