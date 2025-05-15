using BluRayLib;
using BluRayLib.Decrypt;
using BluRayLib.Enums;
using BluRayLib.Mpls;
using MediaLib.BluRays.Exporter;
using MediaLib.BluRays.Sources;
using MediaLib.Models;
using MediaLib.Providers;
using MediaLib.Sources;
using Microsoft.Extensions.Logging;

namespace MediaLib.BluRays.Providers;

/// <summary>
/// A media provider from a BluRay disk.
/// </summary>
public class BluRayMediaProvider : IMediaProvider
{
    private readonly ILogger _logger;
    
    /// <summary>
    /// Gets the BluRay disk information.
    /// </summary>
    public BluRay BluRay { get; }

    public BluRayMediaProvider(ILogger logger, string path)
    {
        _logger = logger;
        BluRay = new BluRay(path);
    }

    static BluRayMediaProvider()
    {
        // Use MakeMkv as a decryption handler. I might add native AACS with a key-config file later.
        MakeMkv.RegisterAsDecryptionHandler();
        MakeMkv.RegisterLibraryImportResolver();
    }

    /// <inheritdoc />
    public async Task<List<IMediaSource>> GetSourcesAsync()
    {
        await BluRay.LoadAsync();

        var sources = new List<BluRayMediaSource>();
        foreach (var playlistId in BluRay.Playlists.Keys.Order())
        {
            var definition = GetSource(playlistId);
            sources.Add(definition);
        }

        for (var a = 0; a < sources.Count; a++)
        for (var b = a + 1; b < sources.Count; b++)
        {
            var sourceA = sources[a];
            var sourceB = sources[b];

            if (sourceA.Info.Matches(sourceB.Info))
            {
                sourceB.Info.IgnoreFlags |= MediaIgnoreFlags.Duplicate;
            }
        }
        
        return sources.Cast<IMediaSource>().ToList();
    }

    /// <summary>
    /// Returns the media source for the given playlist id.
    /// </summary>
    /// <param name="playlistId">The BluRay playlist id.</param>
    /// <returns></returns>
    public BluRayMediaSource GetSource(ushort playlistId)
    {
        var playlist = BluRay.Playlists[playlistId];
        var mediaInfo = new MediaInfo(playlistId);
        
        // Build segments
        var playlistDuration = TimeSpan.Zero;
        var segmentInfos = new List<SegmentInfo>();
        foreach (var item in playlist.Items)
        {
            if (!ushort.TryParse(item.Name, out var clipId))
                continue;
            
            // Video streams
            var first = true;
            var videoStreamInfos = new List<VideoInfo>();
            foreach (var stream in item.StreamNumberTable.PrimaryVideoStreams)
            {
                if (stream.Entry.RefToStreamId == 0) continue;
                var streamInfo = new VideoInfo(stream.Entry.RefToStreamId, GetDescriptionFromStream(stream))
                {
                    IsDefault = first
                };
                videoStreamInfos.Add(streamInfo);
                first = false;
            }
            foreach (var stream in item.StreamNumberTable.SecondaryVideoStream)
            {
                if (stream.Entry.RefToStreamId == 0) continue;
                var streamInfo = new VideoInfo(stream.Entry.RefToStreamId, GetDescriptionFromStream(stream))
                {
                    IsSecondary = true,
                    IsDefault = first
                };
                videoStreamInfos.Add(streamInfo);
                first = false;
            }

            // Audio streams
            first = true;
            var audioStreamInfos = new List<AudioInfo>();
            foreach (var stream in item.StreamNumberTable.PrimaryAudioStreams)
            {
                if (stream.Entry.RefToStreamId == 0) continue;
                var streamInfo = new AudioInfo(stream.Entry.RefToStreamId, GetDescriptionFromStream(stream))
                {
                    LanguageCode = stream.Attributes.LanguageCode,
                    IsDefault = first
                };
                audioStreamInfos.Add(streamInfo);
                first = false;
            }
            foreach (var stream in item.StreamNumberTable.SecondaryAudioStream)
            {
                if (stream.Entry.RefToStreamId == 0) continue;
                var streamInfo = new AudioInfo(stream.Entry.RefToStreamId, GetDescriptionFromStream(stream))
                {
                    LanguageCode = stream.Attributes.LanguageCode,
                    IsSecondary = true,
                    IsDefault = first
                };
                audioStreamInfos.Add(streamInfo);
                first = false;
            }
            
            // Subtitle streams
            first = true;
            var subtitleStreamInfos = new List<SubtitleInfo>();
            foreach (var stream in item.StreamNumberTable.PrimaryPgStreams)
            {
                if (stream.Entry.RefToStreamId == 0) continue;
                var streamInfo = new SubtitleInfo(stream.Entry.RefToStreamId, GetDescriptionFromStream(stream))
                {
                    LanguageCode = stream.Attributes.LanguageCode,
                    IsDefault = first
                };
                subtitleStreamInfos.Add(streamInfo);
                first = false;
            }
            foreach (var stream in item.StreamNumberTable.SecondaryPgStream)
            {
                if (stream.Entry.RefToStreamId == 0) continue;
                var streamInfo = new SubtitleInfo(stream.Entry.RefToStreamId, GetDescriptionFromStream(stream))
                {
                    LanguageCode = stream.Attributes.LanguageCode,
                    IsSecondary = true,
                    IsDefault = first
                };
                subtitleStreamInfos.Add(streamInfo);
                first = false;
            }
                
            var segmentInfo = new SegmentInfo(clipId)
            {
                Duration = TimeSpanFromBluRayTime(item.Duration),
                VideoStreams = videoStreamInfos.ToArray(),
                AudioStreams = audioStreamInfos.ToArray(),
                SubtitleStreams = subtitleStreamInfos.ToArray(),
            };
            playlistDuration += segmentInfo.Duration;
            segmentInfos.Add(segmentInfo);
        }
        mediaInfo.Duration = playlistDuration;
        mediaInfo.Segments = segmentInfos.ToArray();
        
        // Build chapters
        var chapterInfos = new List<ChapterInfo>();
        var chapterTimestamps = new List<TimeSpan>();
        foreach (var mark in playlist.Marks)
        {
            // Adding all previous durations
            uint offset = 0;
            for (var i = 0; i < mark.PlayItemId; i++)
            {
                offset += playlist.Items[i].Duration;
            }
            var item = playlist.Items[mark.PlayItemId];
            var timestamp = TimeSpanFromBluRayTime(offset + mark.TimeStamp - item.InTime);
            // Avoid negative
            if (timestamp < TimeSpan.Zero) timestamp = TimeSpan.Zero;
            // Avoid timestamp above max length. Also add three second tolerance.
            if (timestamp > playlistDuration - TimeSpan.FromSeconds(3)) timestamp = playlistDuration;
            chapterTimestamps.Add(timestamp);
        }
        chapterTimestamps.Add(playlistDuration); // Make sure to add the end of the video.
        for (var i = 0; i < chapterTimestamps.Count - 1; i++)
        {
            var start = chapterTimestamps[i];
            var end = chapterTimestamps[i + 1];
            if (start == end) continue;
            var chapterInfo = new ChapterInfo()
            {
                Index = chapterInfos.Count,
                Name = $"Chapter {chapterInfos.Count+1:00}",
                Start = start,
                End = end
            };
            chapterInfos.Add(chapterInfo);
        }
        mediaInfo.Chapters = chapterInfos.ToArray();

        // Check ignore flags
        var flags = MediaIgnoreFlags.None;

        // Smaller than 10 seconds
        if (mediaInfo.Duration.TotalSeconds < 10) 
        {
            flags |= MediaIgnoreFlags.TooShort;
        }
        
        // Longer than 5 hours
        if (mediaInfo.Duration.TotalSeconds > 60 * 60 * 5) 
        {
            flags |= MediaIgnoreFlags.TooLong;
        }

        // Scan segments
        var audioStreams = 0;
        var subtitleStreams = 0;
        foreach (var segment in mediaInfo.Segments)
        {
            audioStreams += segment.AudioStreams.Length;
            subtitleStreams += segment.SubtitleStreams.Length;
        }
        if (audioStreams == 0)
        {
            flags |= MediaIgnoreFlags.NoAudio;
        }
        if (subtitleStreams == 0)
        {
            flags |= MediaIgnoreFlags.NoSubtitle;
        }
        
        mediaInfo.IgnoreFlags = flags;
        
        var source = new BluRayMediaSource(BluRay, mediaInfo, playlistId);
        return source;
    }

    /// <inheritdoc />
    public IMediaConverter CreateConverter(MediaConverterParameter parameter)
    {
        return new BluRayMediaConverter(_logger, this, parameter);
    }

    /// <inheritdoc />
    public Stream GetRawStream(IMediaSource source)
    {
        if (!Contains(source.Identifier)) throw new ArgumentException($"The given source isn't contained by this provider.", nameof(source));
        var playlist = BluRay.Playlists[source.Identifier.Id];
        var segmentId = ushort.Parse(playlist.Items[0].Name);
        return BluRay.GetM2TsStream(segmentId);
    }

    /// <inheritdoc />
    public bool Contains(MediaIdentifier identifier)
    {
        return identifier.Type == MediaIdentifierType.BluRay && identifier.ContentHash == BluRay.ContentHash;
    }

    #region Utils
    
    /// <summary>
    /// Returns a description of a BluRay stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <returns>Returns the description as text.</returns>
    private static string GetDescriptionFromStream(PlaylistStream stream)
    {
        return stream.Attributes.CodingType switch
        {
            // Video
            StreamCodingType.MPEG1VideoStream => "MPEG1",
            StreamCodingType.MPEG2VideoStream => "MPEG2",
            StreamCodingType.MPEG4AVCVideoStream => "MPEG4 AVC",
            StreamCodingType.MPEG4MVCVideoStream => "MPEG4 MVC",
            StreamCodingType.SMTPEVC1VideoStream => "SMTPEVC1",
            StreamCodingType.HEVCVideoStream => "HEVC",
            // Audio
            StreamCodingType.MPEG1AudioStream => "MPEG1",
            StreamCodingType.MPEG2AudioStream => "MPEG2",
            StreamCodingType.LPCMAudioStream => "LPCM",
            StreamCodingType.DolbyDigitalAudioStream => "DolbyDigital",
            StreamCodingType.DtsAudioStream => "DTS",
            StreamCodingType.DolbyDigitalTrueHDAudioStream => "Dolby TrueHD",
            StreamCodingType.DolbyDigitalPlusAudioStream => "Dolby Digital Plus",
            StreamCodingType.DtsHDHighResolutionAudioStream => "DTS HD",
            StreamCodingType.DtsHDMasterAudioStream => "DTS HD Master",
            StreamCodingType.DolbyDigitalPlusSecondaryAudioStream => "Dolby Digital Plus (secondary)",
            StreamCodingType.DtsHDSecondaryAudioStream => "DTS HD (secondary)",
            // Subtitle
            StreamCodingType.PresentationGraphicsStream => "PGS",
            StreamCodingType.InteractiveGraphicsStream => "IGS",
            StreamCodingType.TextSubtitleStream => "STR",
            _ => "Stream"
        };
    }
    
    /// <summary>
    /// Converts the BluRay ticks to TimeSpan.
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static TimeSpan TimeSpanFromBluRayTime(uint time)
    {
        return TimeSpan.FromSeconds(time / (double)45000);
    }
    
    #endregion Utils

    #region Dispose
    
    /// <inheritdoc />
    public void Dispose()
    {
        // Nothing to do
    }
    
    #endregion Dispose
}