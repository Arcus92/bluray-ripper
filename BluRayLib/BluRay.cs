using BluRayLib.Clip;
using BluRayLib.Movie;
using BluRayLib.Mpls;
using BluRayLib.Utils;

namespace BluRayLib;

public class BluRay
{
    /// <summary>
    /// Gets the BluRay disk path.
    /// </summary>
    public string DiskPath { get; }

    /// <summary>
    /// Gets the BluRay disk name.
    /// </summary>
    public string DiskName { get; }
    
    public BluRay(string diskPath)
    {
        diskPath = Path.GetFullPath(diskPath).TrimEnd('/', '\\'); // Sanitize
        DiskPath = diskPath;
        DiskName = Path.GetFileName(diskPath);
    }
    
    #region Info
    
    /// <summary>
    /// Gets if the BluRay is encrypted.
    /// </summary>
    public bool IsEncrypted { get; private set; }
    
    /// <summary>
    /// Gets the content hash of the disc. This content hash is compatible with TheDiscDb.
    /// </summary>
    public string ContentHash { get; private set; } = "";

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
            
            var fileInfos = new List<ContentHash.HashFileInfo>();
            
            // Load the playlist from the directory
            path = Path.Combine(DiskPath, "BDMV/PLAYLIST/");
            foreach (var file in Directory.EnumerateFiles(path, "*.mpls"))
            {
                var name = Path.GetFileNameWithoutExtension(file);
                if (!ushort.TryParse(name, out var id)) continue;
                
                // Collect infos for the content hash calculation
                var fileInfo = new FileInfo(file);
                fileInfos.Add(new ContentHash.HashFileInfo()
                {
                    Name = name,
                    CreationTime = fileInfo.CreationTime,
                    Size = fileInfo.Length,
                });
                
                // Read the playlist file
                var item = new Playlist();
                item.Read(file);
                Playlists.Add(id, item);
            }
            
            // Order the playlist files before calculating the hash.
            ContentHash = fileInfos.OrderBy(i => i.Name).CalculateHash();
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
    
    /// <summary>
    /// Returns the file info for the M2TS file.
    /// </summary>
    /// <param name="clipId"></param>
    /// <returns></returns>
    public FileInfo GetM2TsFileInfo(ushort clipId)
    {
        var path = Path.Combine(DiskPath, "BDMV/STREAM", $"{clipId:00000}.m2ts");
        return new FileInfo(path);
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