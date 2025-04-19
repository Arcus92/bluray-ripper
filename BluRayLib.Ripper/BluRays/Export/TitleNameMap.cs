namespace BluRayLib.Ripper.BluRays.Export;

/// <summary>
/// A filename map for a title and all sup-files, like subtitles mapped by stream id.
/// The stream id 0 is the main video file.
/// </summary>
public class TitleNameMap : Dictionary<ushort, string>
{
}