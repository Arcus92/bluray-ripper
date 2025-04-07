using BluRayLib.Utils.IO;

namespace BluRayLib.Mpls;

public class StnTable
{
    /// <summary>
    /// Gets and sets the primary video streams.
    /// </summary>
    public PlaylistStream[] PrimaryVideoStreams { get; set; } = [];
    
    /// <summary>
    /// Gets and sets the primary audio streams.
    /// </summary>
    public PlaylistStream[] PrimaryAudioStreams { get; set; } = [];
    
    /// <summary>
    /// Gets and sets the primary presentation graphics streams.
    /// </summary>
    public PlaylistStream[] PrimaryPgStreams { get; set; } = [];
    
    /// <summary>
    /// Gets and sets the primary interactive graphics streams.
    /// </summary>
    public PlaylistStream[] PrimaryIgStreams { get; set; } = [];
    
    /// <summary>
    /// Gets and sets the secondary video streams.
    /// </summary>
    public PlaylistStream[] SecondaryVideoStream { get; set; } = [];
    
    /// <summary>
    /// Gets and sets the secondary audio streams.
    /// </summary>
    public PlaylistStream[] SecondaryAudioStream { get; set; } = [];
    
    /// <summary>
    /// Gets and sets the secondary presentation graphics streams.
    /// </summary>
    public PlaylistStream[] SecondaryPgStream { get; set; } = [];
    
    /// <summary>
    /// Gets and sets the dv streams.
    /// </summary>
    public PlaylistStream[] DvStream { get; set; } = [];

    public void Read(BigEndianBinaryReader reader)
    {
        var length = reader.ReadUInt16();
        var start = reader.Position;
        if (length == 0) return;
        
        reader.Skip(2); // Reserved
        var primaryVideoStreamCount = reader.ReadByte();
        var primaryAudioStreamCount = reader.ReadByte();
        var primaryPgStreamCount = reader.ReadByte();
        var primaryIgStreamCount = reader.ReadByte();
        var secondaryAudioStreamCount = reader.ReadByte();
        var secondaryVideoStreamCount = reader.ReadByte();
        var secondaryPgStreamCount = reader.ReadByte();
        var dvStreamCount = reader.ReadByte();

        reader.Skip(4); // Reserved
        
        PrimaryVideoStreams = new PlaylistStream[primaryVideoStreamCount];
        for (var i = 0; i < primaryVideoStreamCount; i++)
        {
            var streamInfo = new PlaylistStream();
            streamInfo.Read(reader);
            PrimaryVideoStreams[i] = streamInfo;
        }
        
        PrimaryAudioStreams = new PlaylistStream[primaryAudioStreamCount];
        for (var i = 0; i < primaryAudioStreamCount; i++)
        {
            var streamInfo = new PlaylistStream();
            streamInfo.Read(reader);
            PrimaryAudioStreams[i] = streamInfo;
        }
        
        PrimaryPgStreams = new PlaylistStream[primaryPgStreamCount];
        for (var i = 0; i < primaryPgStreamCount; i++)
        {
            var streamInfo = new PlaylistStream();
            streamInfo.Read(reader);
            PrimaryPgStreams[i] = streamInfo;
        }
        
        PrimaryIgStreams = new PlaylistStream[primaryIgStreamCount];
        for (var i = 0; i < primaryIgStreamCount; i++)
        {
            var streamInfo = new PlaylistStream();
            streamInfo.Read(reader);
            PrimaryIgStreams[i] = streamInfo;
        }
        
        SecondaryAudioStream = new PlaylistStream[secondaryAudioStreamCount];
        for (var i = 0; i < secondaryAudioStreamCount; i++)
        {
            var streamInfo = new PlaylistStream();
            streamInfo.Read(reader);
            SecondaryAudioStream[i] = streamInfo;
        }
        
        SecondaryVideoStream = new PlaylistStream[secondaryVideoStreamCount];
        for (var i = 0; i < secondaryVideoStreamCount; i++)
        {
            var streamInfo = new PlaylistStream();
            streamInfo.Read(reader);
            SecondaryVideoStream[i] = streamInfo;
        }

        SecondaryPgStream = new PlaylistStream[secondaryPgStreamCount];
        for (var i = 0; i < secondaryPgStreamCount; i++)
        {
            var streamInfo = new PlaylistStream();
            streamInfo.Read(reader);
            SecondaryPgStream[i] = streamInfo;
        }
        
        DvStream = new PlaylistStream[dvStreamCount];
        for (var i = 0; i < dvStreamCount; i++)
        {
            var streamInfo = new PlaylistStream();
            streamInfo.Read(reader);
            DvStream[i] = streamInfo;
        }
        
        reader.SkipTo(start + length);
    }
}