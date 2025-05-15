namespace MediaLib.FFmpeg;

/// <summary>
/// Helper class for <see cref="ConverterUpdate"/>.
/// </summary>
public static class ConverterUpdateHelper
{
    /// <summary>
    /// Returns a readable progress text from the converter update.
    /// </summary>
    /// <param name="update"></param>
    /// <returns></returns>
    public static string GetProgressText(this ConverterUpdate update)
    {
        var percentage = update.Percentage.HasValue ? $"{Math.Floor(update.Percentage.Value * 100):0}%" : "N/A";
        var current = update.Current.HasValue ? $@"{update.Current.Value:hh\:mm\:ss}" : "N/A";
        var duration = update.Duration.HasValue ? $@"{update.Duration.Value:hh\:mm\:ss}" : "N/A";
        
        return $"[{percentage}] {current} / {duration}";
    }
}