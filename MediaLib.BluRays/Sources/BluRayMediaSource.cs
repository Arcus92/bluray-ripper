using System.Text;
using BluRayLib;
using MediaLib.BluRays.Providers;
using MediaLib.Models;
using MediaLib.Output;
using MediaLib.Sources;

namespace MediaLib.BluRays.Sources;

public class BluRayMediaSource : IMediaSource
{
    private readonly BluRay _bluRay;

    public BluRayMediaSource(BluRay bluRay, MediaInfo info, ushort playlistId)
    {
        _bluRay = bluRay;
        Identifier = new MediaIdentifier()
        {
            Type = MediaIdentifierType.BluRay,
            ContentHash = bluRay.ContentHash,
            DiskName = bluRay.DiskName,
            Id = playlistId
        };
        Info = info;
        PlaylistId = playlistId;
    }

    #region Media info
    
    /// <inheritdoc />
    public MediaIdentifier Identifier { get; }
    
    /// <inheritdoc />
    public MediaInfo Info { get; }

    /// <summary>
    /// Gets the playlist id.
    /// </summary>
    public ushort PlaylistId { get; }

    #endregion Media info

    #region Output
    
    /// <inheritdoc />
    public OutputDefinition CreateDefaultOutputDefinition(CodecOptions codec, VideoFormat format)
    {
        var playlist = _bluRay.Playlists[PlaylistId];
        if (playlist.Items.Length == 0)
            throw new ArgumentException("Cannot create output for title without segments!", nameof(PlaylistId));

        var baseName = $"{_bluRay.DiskName}_{PlaylistId}";
        var segment = playlist.Items[0];
        var exportSubtitlesAsSeparateFiles = !format.SupportPgs;
        
        // Calculate total duration
        var duration = TimeSpan.Zero;
        foreach (var item in playlist.Items)
        {
            duration += BluRayMediaProvider.TimeSpanFromBluRayTime(item.Duration);
        }
        
        // Collect all streams
        var streams = new List<OutputStream>();
        var first = true;
        
        // Video streams
        foreach (var stream in segment.StreamNumberTable.PrimaryVideoStreams)
        {
            if (stream.Entry.RefToStreamId == 0) continue;
            streams.Add(new OutputStream()
            {
                Id = stream.Entry.RefToStreamId,
                Type = OutputStreamType.Video,
                Default = first,
            });
            first = false;
        }
        foreach (var stream in segment.StreamNumberTable.SecondaryVideoStream)
        {
            if (stream.Entry.RefToStreamId == 0) continue;
            streams.Add(new OutputStream()
            {
                Id = stream.Entry.RefToStreamId,
                Type = OutputStreamType.Video,
                Default = first,
            });
            first = false;
        }
        
        // Audio streams
        first = true;
        foreach (var stream in segment.StreamNumberTable.PrimaryAudioStreams)
        {
            if (stream.Entry.RefToStreamId == 0) continue;
            streams.Add(new OutputStream()
            {
                Id = stream.Entry.RefToStreamId,
                Type = OutputStreamType.Audio,
                LanguageCode = stream.Attributes.LanguageCode,
                Default = first,
            });
            first = false;
        }
        foreach (var stream in segment.StreamNumberTable.SecondaryAudioStream)
        {
            if (stream.Entry.RefToStreamId == 0) continue;
            streams.Add(new OutputStream()
            {
                Id = stream.Entry.RefToStreamId,
                Type = OutputStreamType.Audio,
                LanguageCode = stream.Attributes.LanguageCode,
                Default = first,
            });
            first = false;
        }
        
        // Subtitles
        first = true;
        foreach (var stream in segment.StreamNumberTable.PrimaryPgStreams)
        {
            if (stream.Entry.RefToStreamId == 0) continue;
            streams.Add(new OutputStream()
            {
                Id = stream.Entry.RefToStreamId,
                Type = OutputStreamType.Subtitle,
                LanguageCode = stream.Attributes.LanguageCode,
                Default = first,
            });
            first = false;
        }
        foreach (var stream in segment.StreamNumberTable.SecondaryPgStream)
        {
            if (stream.Entry.RefToStreamId == 0) continue;
            streams.Add(new OutputStream()
            {
                Id = stream.Entry.RefToStreamId,
                Type = OutputStreamType.Subtitle,
                LanguageCode = stream.Attributes.LanguageCode,
                Default = first,
            });
            first = false;
        }
        
        
        var files = new List<OutputFile>();
        
        // Create the main video file.
        var mainFile = new OutputFile()
        {
            Filename = $"{baseName}{format.Extension}",
            Format = format.FFmpegFormat,
        };
        if (exportSubtitlesAsSeparateFiles)
        {
            // Only contains video and audio
            mainFile.Streams = streams.Where(s => s.Type is OutputStreamType.Video or OutputStreamType.Audio).ToArray();
        }
        else
        {
            // Contains all streams
            mainFile.Streams = streams.ToArray();
        }
        files.Add(mainFile);
        
        
        // Adds subtitle files
        if (exportSubtitlesAsSeparateFiles)
        {
            // Guessing the subtitle type by order.
            var languageCounter = new Dictionary<string, int>();
            var filename = new StringBuilder();
            
            foreach (var stream in streams.Where(s => s.Type is OutputStreamType.Subtitle))
            {
                // Count how often the language code was encountered.
                var languageCode = stream.LanguageCode ?? "";
                languageCounter.TryGetValue(languageCode, out var counter);

                // Building the filename
                filename.Clear();
                filename.Append(baseName);

                if (!string.IsNullOrEmpty(languageCode))
                {
                    filename.Append('.');
                    filename.Append(languageCode);
                }
                
                // Second subtitle. Assume: forced subtitle track.
                if (counter == 1)
                {
                    filename.Append(".forced");
                }
                else if (counter >= 2) // Extra subtitles
                {
                    filename.Append($".extra{counter - 1}");
                }
                filename.Append(".sup");
                
                // Increment counter
                languageCounter[languageCode] = ++counter;
                
                files.Add(new OutputFile()
                {
                    Filename = filename.ToString(),
                    Format = "sup",
                    Streams = [stream]
                });
            }
        }
        
        // Build chapters
        var chapterInfos = new List<OutputChapter>();
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
            var timestamp = BluRayMediaProvider.TimeSpanFromBluRayTime(offset + mark.TimeStamp - item.InTime);
            // Avoid negative
            if (timestamp < TimeSpan.Zero) timestamp = TimeSpan.Zero;
            // Avoid timestamp above max length. Also add three-second tolerances.
            if (timestamp > duration - TimeSpan.FromSeconds(3)) timestamp = duration;
            chapterTimestamps.Add(timestamp);
        }
        chapterTimestamps.Add(duration); // Make sure to add the end of the video.
        for (var i = 0; i < chapterTimestamps.Count - 1; i++)
        {
            var start = chapterTimestamps[i];
            var end = chapterTimestamps[i + 1];
            if (start == end) continue;
            var chapterInfo = new OutputChapter()
            {
                Name = $"Chapter {chapterInfos.Count+1:00}",
                Start = start,
                End = end
            };
            chapterInfos.Add(chapterInfo);
        }
        
        return new OutputDefinition()
        {
            Identifier = Identifier,
            MediaInfo = new OutputMediaInfo()
            {
                Name = baseName,
            },
            Duration = duration,
            Codec = codec,
            Files = files.ToArray(),
            Chapters = chapterInfos.ToArray()
        };
    }
    
    #endregion Output
}