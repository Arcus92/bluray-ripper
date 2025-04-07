using BluRayLib.Enums;
using BluRayLib.Mpls;

namespace BluRayLib.Ripper.Info;

public static class BluRayInfo
{
    /// <summary>
    /// Returns the info data for the given playlist id.
    /// </summary>
    /// <param name="bluRay">The BluRay instance.</param>
    /// <param name="playlistId">The playlist id.</param>
    /// <returns></returns>
    public static PlaylistInfo GetPlaylistInfo(BluRay bluRay, ushort playlistId)
    {
        var playlist = bluRay.Playlists[playlistId];
        var playlistInfo = new PlaylistInfo(playlistId);

        // Build segments
        var playlistDuration = TimeSpan.Zero;
        var segmentInfos = new List<SegmentInfo>();
        foreach (var item in playlist.Items)
        {
            if (!ushort.TryParse(item.Name, out var clipId))
                continue;
            
            var clip = bluRay.Clips[clipId];
            var pidToIndex = new Dictionary<ushort, int>();
            foreach (var program in clip.Programs)
            {
                // Order by Pid to match the FFmpeg stream order
                foreach (var stream in program.Streams.OrderBy(s => s.Pid))
                {
                    pidToIndex.Add(stream.Pid, pidToIndex.Count);
                }
            }
            
            // Video streams
            var videoStreamInfos = new List<VideoInfo>();
            foreach (var stream in item.StnTable.PrimaryVideoStreams)
            {
                if (stream.Entry.RefToStreamId == 0) continue;
                var streamInfo = new VideoInfo(stream.Entry.RefToStreamId)
                {
                    Index = pidToIndex[stream.Entry.RefToStreamId],
                };
                videoStreamInfos.Add(streamInfo);
            }
            foreach (var stream in item.StnTable.SecondaryVideoStream)
            {
                if (stream.Entry.RefToStreamId == 0) continue;
                var streamInfo = new VideoInfo(stream.Entry.RefToStreamId)
                {
                    Index = pidToIndex[stream.Entry.RefToStreamId],
                    IsSecondary = true
                };
                videoStreamInfos.Add(streamInfo);
            }

            // Audio streams
            var audioStreamInfos = new List<AudioInfo>();
            foreach (var stream in item.StnTable.PrimaryAudioStreams)
            {
                if (stream.Entry.RefToStreamId == 0) continue;
                var streamInfo = new AudioInfo(stream.Entry.RefToStreamId)
                {
                    Index = pidToIndex[stream.Entry.RefToStreamId],
                    LanguageCode = stream.Attributes.LanguageCode
                };
                audioStreamInfos.Add(streamInfo);
            }
            foreach (var stream in item.StnTable.SecondaryAudioStream)
            {
                if (stream.Entry.RefToStreamId == 0) continue;
                var streamInfo = new AudioInfo(stream.Entry.RefToStreamId)
                {
                    Index = pidToIndex[stream.Entry.RefToStreamId],
                    LanguageCode = stream.Attributes.LanguageCode,
                    IsSecondary = true
                };
                audioStreamInfos.Add(streamInfo);
            }
            
            // Subtitle streams
            var subtitleStreamInfos = new List<SubtitleInfo>();
            foreach (var stream in item.StnTable.PrimaryPgStreams)
            {
                if (stream.Entry.RefToStreamId == 0) continue;
                var streamInfo = new SubtitleInfo(stream.Entry.RefToStreamId)
                {
                    Index = pidToIndex[stream.Entry.RefToStreamId],
                    LanguageCode = stream.Attributes.LanguageCode
                };
                subtitleStreamInfos.Add(streamInfo);
            }
            foreach (var stream in item.StnTable.SecondaryPgStream)
            {
                if (stream.Entry.RefToStreamId == 0) continue;
                var streamInfo = new SubtitleInfo(stream.Entry.RefToStreamId)
                {
                    Index = pidToIndex[stream.Entry.RefToStreamId],
                    LanguageCode = stream.Attributes.LanguageCode,
                    IsSecondary = true
                };
                subtitleStreamInfos.Add(streamInfo);
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
        var flags = PlaylistIgnoreFlags.None;

        // Smaller than 10 seconds
        if (playlistInfo.Duration.TotalSeconds < 10) 
        {
            flags |= PlaylistIgnoreFlags.TooShort;
        }
        
        // Longer than 5 hours
        if (playlistInfo.Duration.TotalSeconds > 60 * 60 * 5) 
        {
            flags |= PlaylistIgnoreFlags.TooLong;
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
            flags |= PlaylistIgnoreFlags.NoAudio;
        }
        if (subtitleStreams == 0)
        {
            flags |= PlaylistIgnoreFlags.NoSubtitle;
        }


        
        playlistInfo.IgnoreFlags = flags;
        
        return playlistInfo;
    }

    /// <summary>
    /// Returns playlist info data for each playlist on the BluRay disc.
    /// </summary>
    /// <param name="bluRay">The BluRay instance.</param>
    /// <returns></returns>
    public static PlaylistInfo[] GetPlaylistInfos(BluRay bluRay)
    {
        var playlistInfos = new List<PlaylistInfo>();
        foreach (var playlistId in bluRay.Playlists.Keys.Order())
        {
            playlistInfos.Add(GetPlaylistInfo(bluRay, playlistId));
        }

        for (var a = 0; a < playlistInfos.Count; a++)
        for (var b = a + 1; b < playlistInfos.Count; b++)
        {
            var playlistA = playlistInfos[a];
            var playlistB = playlistInfos[b];

            if (playlistA.Matches(playlistB))
            {
                playlistB.IgnoreFlags |= PlaylistIgnoreFlags.Duplicate;
            }
        }
        
        return playlistInfos.ToArray();
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