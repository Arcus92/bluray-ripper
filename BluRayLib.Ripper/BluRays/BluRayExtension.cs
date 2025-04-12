using BluRayLib.Ripper.BluRays.Export;

namespace BluRayLib.Ripper.BluRays;

public static class BluRayExtension
{
    /// <inheritdoc cref="BluRayData.GetTitle"/>
    public static TitleData GetTitle(this BluRay bluRay, ushort playlistId)
    {
        return BluRayData.GetTitle(bluRay, playlistId);
    }
    
    /// <inheritdoc cref="BluRayData.GetTitles"/>
    public static TitleData[] GetTitles(this BluRay bluRay)
    {
        return BluRayData.GetTitles(bluRay);
    }
}