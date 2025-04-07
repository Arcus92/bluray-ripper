using BluRayLib.Enums;
using BluRayLib.Utils.IO;

namespace BluRayLib.Mpls;

/// <summary>
/// A BluRay playlist file.
/// </summary>
public class Playlist
{
    // https://github.com/lw/BluRay/wiki/STNTable
    
    /// <summary>
    /// Gets and sets the playback type.
    /// </summary>
    public PlaybackType PlaybackType { get; set; }
    
    /// <summary>
    /// Gets and sets the playback count.
    /// </summary>
    public ushort PlaybackCount { get; set; }

    /// <summary>
    /// Gets and sets the mask table.
    /// </summary>
    public MaskTable MaskTable { get; } = new();
    
    /// <summary>
    /// Gets and sets the misc flags.
    /// </summary>
    public MiscFlag MiscFlag { get; set; }
    
    /// <summary>
    /// Reads the playlist file.
    /// </summary>
    /// <param name="path">The path to the playlist file.</param>
    public void Read(string path)
    {
        using var fileStream = File.OpenRead(path);
        Read(fileStream);
    }
    
    /// <summary>
    /// Reads the playlist file from stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    public void Read(Stream stream)
    {
        var reader = new BigEndianBinaryReader(stream);
        Read(reader);
    }

    private void Read(BigEndianBinaryReader reader)
    {
        // https://en.wikibooks.org/wiki/User:Bdinfo/mpls
        
        var format = reader.ReadString(4);
        if (format != "MPLS")
            throw new InvalidDataException("Invalid playlist magic number!");
        var version = reader.ReadString(4);
        if (version != "0200")
            throw new InvalidDataException($"Invalid playlist version: {version}!");

        var playlistStart = reader.ReadUInt32();
        var playlistMarkStart = reader.ReadUInt32();
        var extensionDataStart = reader.ReadUInt32();
        reader.Skip(20);
        
        var length = reader.ReadUInt32();
        reader.Skip(1); // Reserved
        PlaybackType = (PlaybackType)reader.ReadByte();
        PlaybackCount = 0;
        if (PlaybackType != PlaybackType.Standard)
        {
            PlaybackCount = reader.ReadUInt16();
        }

        MaskTable.Read(reader);
        MiscFlag = (MiscFlag)reader.ReadByte();
        reader.Skip(1); // Reserved


        reader.Position = playlistStart;
        ReadPlaylist(reader);

        reader.Position = playlistMarkStart;
        ReadMark(reader);
    }
    
    #region Playlist
    
    /// <summary>
    /// Gets and sets the playlist items.
    /// </summary>
    public PlaylistItem[] Items { get; set; } = [];
    
    /// <summary>
    /// Gets and sets the playlist sub-paths.
    /// </summary>
    public SubPath[] SubPaths { get; set; } = [];
    
    private void ReadPlaylist(BigEndianBinaryReader reader)
    {
        var length = reader.ReadUInt32();
        var start = reader.Position;
        
        reader.Skip(2);
        var itemCount = reader.ReadUInt16();
        var subPathCount = reader.ReadUInt16();
        
        Items = new PlaylistItem[itemCount];
        for (var i = 0; i < itemCount; i++)
        {
            var item = new PlaylistItem();
            item.Read(reader);
            Items[i] = item;
        }
        
        SubPaths = new SubPath[subPathCount];
        for (var i = 0; i < subPathCount; i++)
        {
            var subPath = new SubPath();
            subPath.Read(reader);
            SubPaths[i] = subPath;
        }
        
        reader.SkipTo(start + length);
    }
    
    #endregion Playlist
    
    #region Mark

    /// <summary>
    /// Gets and sets the marks.
    /// </summary>
    public PlaylistMark[] Marks { get; set; } = [];
    
    private void ReadMark(BigEndianBinaryReader reader)
    {
        var length = reader.ReadUInt32();
        var markCount = reader.ReadUInt16();
        Marks = new PlaylistMark[markCount];
        for (var i = 0; i < markCount; i++)
        {
            var mark = new PlaylistMark();
            mark.Read(reader);
            Marks[i] = mark;
        }
    }

    #endregion Mark 
}