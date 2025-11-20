using DvdLib.Streams;
using DvdLib.Vmg;

namespace DvdLib;

public class Dvd
{
    /// <summary>
    /// Gets the Dvd disk path.
    /// </summary>
    public string DiskPath { get; }

    /// <summary>
    /// Gets the Dvd disk name.
    /// </summary>
    public string DiskName { get; }
    
    /// <summary>
    /// Gets all loaded title set information from VIDEO_TS.
    /// </summary>
    public Dictionary<VmgIdentifier, Ifo> TitleSets { get; } = new();
    
    /// <summary>
    /// Gets all loaded video streams from VIDEO_TS.
    /// </summary>
    public Dictionary<VmgIdentifier, VideoStream> VideoStreams { get; } = new();
    
    public Dvd(string diskPath)
    {
        diskPath = Path.GetFullPath(diskPath).TrimEnd('/', '\\'); // Sanitize
        DiskPath = diskPath;
        DiskName = Path.GetFileName(diskPath);
    }
    
    #region Info
    
    /// <summary>
    /// Gets the content hash of the disc. This content hash is compatible with TheDiscDb.
    /// </summary>
    public string ContentHash { get; private set; } = "";

    /// <summary>
    /// Loads the DVD content and populates <see cref="VideoStreams"/>.
    /// </summary>
    public async Task LoadAsync()
    {
        TitleSets.Clear();
        VideoStreams.Clear();
        await Task.Run(() =>
        {
            var videoStreamPath = Path.Combine(DiskPath, "VIDEO_TS");
            
            var titleSets = Directory.EnumerateFiles(videoStreamPath, "*.IFO");
            foreach (var titleSet in titleSets)
            {
                var filename = Path.GetFileNameWithoutExtension(titleSet);
                var identifier = VmgIdentifier.FromFilename(filename);
                
                var ifo = new Ifo();
                ifo.Read(titleSet);
                TitleSets.Add(identifier, ifo);
            }
            
            var videoStreams = Directory.EnumerateFiles(videoStreamPath, "*.VOB");
            foreach (var videoStream in videoStreams)
            {
                var filename = Path.GetFileNameWithoutExtension(videoStream);
                var identifier = VmgIdentifier.FromFilename(filename);

                var information = TitleSets[identifier.Root];
                var stream = new VideoStream(identifier, information);
                VideoStreams.Add(identifier, stream);
            }
        });
    }
    
    #endregion Info
    
    #region Streams
    
    /// <summary>
    /// Opens the VOB file with the given name.
    /// </summary>
    /// <param name="identifier">The file identifier.</param>
    /// <returns>Returns the stream.</returns>
    public Stream GetVobStream(VmgIdentifier identifier)
    {
        var path = Path.Combine(DiskPath, "VIDEO_TS", $"{identifier.ToFilename()}.VOB");
        
        // Handle decryption
        if (VobDecryptionHandler is not null)
        {
            return VobDecryptionHandler.Invoke(this, path);
        }
        
        return File.OpenRead(path);
    }
    
    /// <summary>
    /// Returns the file info for the VOB file.
    /// </summary>
    /// <param name="identifier">The file identifier.</param>
    /// <returns>Returns the file info.</returns>
    public FileInfo GetVobFileInfo(VmgIdentifier identifier)
    {
        var path = Path.Combine(DiskPath, "VIDEO_TS", $"{identifier.ToFilename()}.VOB");
        return new FileInfo(path);
    }
    
    #endregion Streams
    
    #region Decryption
    
    /// <summary>
    /// The decryption handler method.
    /// </summary>
    public delegate Stream DecryptionHandler(Dvd dvd, string filename);

    /// <summary>
    /// Gets and sets the VOB decryption stream.
    /// </summary>
    public static DecryptionHandler? VobDecryptionHandler { get; set; }

    #endregion Decryption
}