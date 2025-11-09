namespace MediaLib.Formats;

public static class MediaFormatHelper
{
    /// <summary>
    /// Returns the format from the file extension.
    /// </summary>
    /// <param name="formats">The list of formats</param>
    /// <param name="extension">The file extensions.</param>
    /// <param name="result">Returns the format if found.</param>
    /// <returns>Returns true, if the format was found.</returns>
    public static bool TryGetFromExtension(this IEnumerable<MediaFormat> formats, string extension, out MediaFormat result)
    {
        foreach (var format in formats)
        {
            if (!string.Equals(format.Extension, extension, StringComparison.InvariantCultureIgnoreCase)) continue;
            result = format;
            return true;
        }
        result = default;
        return false;
    }
    
    /// <summary>
    /// Returns the format from the FFmpeg format name.
    /// </summary>
    /// <param name="formats">The list of formats</param>
    /// <param name="ffmpegFormat">The FFmpeg format.</param>
    /// <param name="result">Returns the format if found.</param>
    /// <returns>Returns true, if the format was found.</returns>
    public static bool TryGetFromFormat(this IEnumerable<MediaFormat> formats, string ffmpegFormat, out MediaFormat result)
    {
        foreach (var format in formats)
        {
            if (!string.Equals(format.FFmpegFormat, ffmpegFormat, StringComparison.InvariantCultureIgnoreCase)) continue;
            result = format;
            return true;
        }
        result = default;
        return false;
    }
}