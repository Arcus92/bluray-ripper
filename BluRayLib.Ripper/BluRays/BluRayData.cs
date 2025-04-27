using BluRayLib.Enums;
using BluRayLib.Mpls;

namespace BluRayLib.Ripper.BluRays;

public static class BluRayData
{
    /// <summary>
    /// Returns the data for the given playlist id.
    /// </summary>
    /// <param name="bluRay">The BluRay instance.</param>
    /// <param name="playlistId">The playlist id.</param>
    /// <returns></returns>
    public static TitleData GetTitle(BluRay bluRay, ushort playlistId)
    {
        var playlist = bluRay.Playlists[playlistId];
        var playlistInfo = new TitleData(playlistId);

        // Build segments
        var playlistDuration = TimeSpan.Zero;
        var segmentInfos = new List<SegmentData>();
        foreach (var item in playlist.Items)
        {
            if (!ushort.TryParse(item.Name, out var clipId))
                continue;
            
            var clip = bluRay.Clips[clipId];
            
            // Video streams
            var first = true;
            var videoStreamInfos = new List<VideoData>();
            foreach (var stream in item.StreamNumberTable.PrimaryVideoStreams)
            {
                if (stream.Entry.RefToStreamId == 0) continue;
                var streamInfo = new VideoData(stream.Entry.RefToStreamId, GetDescriptionFromStream(stream))
                {
                    IsDefault = first
                };
                videoStreamInfos.Add(streamInfo);
                first = false;
            }
            foreach (var stream in item.StreamNumberTable.SecondaryVideoStream)
            {
                if (stream.Entry.RefToStreamId == 0) continue;
                var streamInfo = new VideoData(stream.Entry.RefToStreamId, GetDescriptionFromStream(stream))
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
            var subtitleStreamInfos = new List<SubtitleData>();
            foreach (var stream in item.StreamNumberTable.PrimaryPgStreams)
            {
                if (stream.Entry.RefToStreamId == 0) continue;
                var streamInfo = new SubtitleData(stream.Entry.RefToStreamId, GetDescriptionFromStream(stream))
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
                var streamInfo = new SubtitleData(stream.Entry.RefToStreamId, GetDescriptionFromStream(stream))
                {
                    LanguageCode = stream.Attributes.LanguageCode,
                    IsSecondary = true,
                    IsDefault = first
                };
                subtitleStreamInfos.Add(streamInfo);
                first = false;
            }
                
            var segmentInfo = new SegmentData(clipId)
            {
                Duration = TimeSpanFromBluRayTime(item.Duration),
                VideoStreams = videoStreamInfos.ToArray(),
                AudioStreams = audioStreamInfos.ToArray(),
                SubtitleStreams = subtitleStreamInfos.ToArray(),
            };
            playlistDuration += segmentInfo.Duration;
            segmentInfos.Add(segmentInfo);
        }
        playlistInfo.Duration = playlistDuration;
        playlistInfo.Segments = segmentInfos.ToArray();
        
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
        playlistInfo.Chapters = chapterInfos.ToArray();

        // Check ignore flags
        var flags = TitleIgnoreFlags.None;

        // Smaller than 10 seconds
        if (playlistInfo.Duration.TotalSeconds < 10) 
        {
            flags |= TitleIgnoreFlags.TooShort;
        }
        
        // Longer than 5 hours
        if (playlistInfo.Duration.TotalSeconds > 60 * 60 * 5) 
        {
            flags |= TitleIgnoreFlags.TooLong;
        }

        // Scan segments
        var audioStreams = 0;
        var subtitleStreams = 0;
        foreach (var segment in playlistInfo.Segments)
        {
            audioStreams += segment.AudioStreams.Length;
            subtitleStreams += segment.SubtitleStreams.Length;
        }
        if (audioStreams == 0)
        {
            flags |= TitleIgnoreFlags.NoAudio;
        }
        if (subtitleStreams == 0)
        {
            flags |= TitleIgnoreFlags.NoSubtitle;
        }


        
        playlistInfo.IgnoreFlags = flags;
        
        return playlistInfo;
    }

    /// <summary>
    /// Returns playlist info data for each playlist on the BluRay disc.
    /// </summary>
    /// <param name="bluRay">The BluRay instance.</param>
    /// <returns></returns>
    public static TitleData[] GetTitles(BluRay bluRay)
    {
        var titles = new List<TitleData>();
        var index = 0;
        foreach (var playlistId in bluRay.Playlists.Keys.Order())
        {
            var title = GetTitle(bluRay, playlistId);
            title.Index = index++;
            titles.Add(title);
        }

        for (var a = 0; a < titles.Count; a++)
        for (var b = a + 1; b < titles.Count; b++)
        {
            var playlistA = titles[a];
            var playlistB = titles[b];

            if (playlistA.Matches(playlistB))
            {
                playlistB.IgnoreFlags |= TitleIgnoreFlags.Duplicate;
            }
        }
        
        return titles.ToArray();
    }
    
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
    private static TimeSpan TimeSpanFromBluRayTime(uint time)
    {
        return TimeSpan.FromSeconds(time / (double)45000);
    }
}