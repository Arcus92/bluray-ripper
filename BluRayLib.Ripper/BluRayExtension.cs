using BluRayLib.Ripper.Export;
using BluRayLib.Ripper.Info;

namespace BluRayLib.Ripper;

public static class BluRayExtension
{
    /// <inheritdoc cref="BluRayInfo.GetPlaylistInfo"/>
    public static PlaylistInfo GetPlaylistInfo(this BluRay bluRay, ushort playlistId)
    {
        return BluRayInfo.GetPlaylistInfo(bluRay, playlistId);
    }
    
    /// <inheritdoc cref="BluRayInfo.GetPlaylistInfos"/>
    public static PlaylistInfo[] GetPlaylistInfos(this BluRay bluRay)
    {
        return BluRayInfo.GetPlaylistInfos(bluRay);
    }

    /// <summary>
    /// Creates an exporter for the given playlist.
    /// </summary>
    /// <param name="bluRay">The BluRay instance.</param>
    /// <param name="playlistId">The playlist instance.</param>
    /// <returns></returns>
    public static PlaylistExporter CreatePlaylistExporter(this BluRay bluRay, ushort playlistId)
    {
        return new PlaylistExporter(bluRay, playlistId);
    }
}