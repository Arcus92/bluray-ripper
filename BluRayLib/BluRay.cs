using BluRayLib.Clip;
using BluRayLib.Movie;
using BluRayLib.Mpls;

namespace BluRayLib;

public class BluRay
{
    /// <summary>
    /// Gets the BluRay disk path.
    /// </summary>
    public string DiskPath { get; }

    public BluRay(string diskPath)
    {
        DiskPath = diskPath;
    }
    
    #region Info
    
    /// <summary>
    /// Gets if the BluRay is encrypted.
    /// </summary>
    public bool IsEncrypted { get; private set; }

    /// <summary>
    /// Gets the loaded movie object.
    /// </summary>
    public MovieObjects MovieObjects { get; } = new();
    
    /// <summary>
    /// Gets all loaded playlists.
    /// </summary>
    public Dictionary<ushort, Playlist> Playlists { get; } = new();
    
    /// <summary>
    /// Gets all loaded clips.
    /// </summary>
    public Dictionary<ushort, ClipInfo> Clips { get; } = new();

    /// <summary>
    /// Loads the BluRay disks content and populates the <see cref="Playlists"/> and <see cref="Clips"/> lists.
    /// </summary>
    public async Task LoadAsync()
    {
        Playlists.Clear();
        Clips.Clear();
        await Task.Run(() =>
        {
            // Check if the disk is encrypted by checking the AACS directory.
            IsEncrypted = Directory.Exists(Path.Combine(DiskPath, "AACS"));
            
            // Load the movie object file
            var path = Path.Combine(DiskPath, "BDMV/MovieObject.bdmv");
            MovieObjects.Read(path);
            
            // Load the clips from the directory
            path = Path.Combine(DiskPath, "BDMV/CLIPINF/");
            foreach (var file in Directory.EnumerateFiles(path, "*.clpi"))
            {
                var name = Path.GetFileNameWithoutExtension(file);
                if (!ushort.TryParse(name, out var id)) continue;
                var item = new ClipInfo();
                item.Read(file);
                Clips.Add(id, item);
            }
            
            // Load the playlist from the directory
            path = Path.Combine(DiskPath, "BDMV/PLAYLIST/");
            foreach (var file in Directory.EnumerateFiles(path, "*.mpls"))
            {
                var name = Path.GetFileNameWithoutExtension(file);
                if (!ushort.TryParse(name, out var id)) continue;
                var item = new Playlist();
                item.Read(file);
                Playlists.Add(id, item);
            }
        });
    }
    
    #endregion Info
    
    #region Streams
    
    /// <summary>
    /// Opens the M2TS file with the given id.
    /// </summary>
    /// <param name="clipId"></param>
    /// <returns></returns>
    public Stream GetM2TsStream(ushort clipId)
    {
        // Handle decryption
        if (IsEncrypted && M2TsDecryptionHandler is not null)
        {
            return M2TsDecryptionHandler.Invoke(this, clipId);
        }
        
        var path = Path.Combine(DiskPath, "BDMV/STREAM", $"{clipId:00000}.m2ts");
        return File.OpenRead(path);
    }
    
    #endregion Streams
    
    #region Clip

    /// <summary>
    /// Reads and returns the clip-info file with the given id.
    /// </summary>
    /// <param name="clipId"></param>
    /// <returns></returns>
    public ClipInfo GetClipInfo(ushort clipId)
    {
        var path = Path.Combine(DiskPath, "BDMV/CLIPINF", $"{clipId:00000}.clpi");
        var clipInfo = new ClipInfo();
        clipInfo.Read(path);
        return clipInfo;
    }
    
    #endregion Clip
    
    #region Playlists

    /// <summary>
    /// Reads and returns the playlist with the given id.
    /// </summary>
    /// <param name="playlistId">The playlist id.</param>
    /// <returns>Returns the playlist.</returns>
    public Playlist GetPlaylist(ushort playlistId)
    {
        var path = Path.Combine(DiskPath, "BDMV/PLAYLIST", $"{playlistId:00000}.mpls");
        var playlist = new Playlist();
        playlist.Read(path);
        return playlist;
    }
    
    #endregion Playlists
    
    #region Decryption
    
    /// <summary>
    /// The decryption handler method.
    /// </summary>
    public delegate Stream DecryptionHandler(BluRay bluRay, ushort clipId);

    /// <summary>
    /// Gets and sets the M2TS decryption stream.
    /// </summary>
    public static DecryptionHandler? M2TsDecryptionHandler { get; set; }

    #endregion Decryption
}