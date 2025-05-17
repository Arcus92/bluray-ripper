using System.Text;
using BluRayLib;
using BluRayLib.Enums;
using BluRayLib.Mpls;
using MediaLib.BluRays.Providers;
using MediaLib.Models;
using MediaLib.Output;
using MediaLib.Sources;

namespace MediaLib.BluRays.Sources;

public class BluRayMediaSource : IMediaSource
{
    /// <summary>
    /// The internal BluRay playlist.
    /// </summary>
    private readonly Playlist _playlist;

    public BluRayMediaSource(Playlist playlist, MediaIdentifier identifier)
    {
        _playlist = playlist;
        Identifier = identifier;
        Info = BuildMediaInfo();
    }

    #region Media info
    
    /// <inheritdoc />
    public MediaIdentifier Identifier { get; }
    
    /// <inheritdoc />
    public MediaInfo Info { get; }
    
    /// <inheritdoc />
    public MediaIgnoreFlags IgnoreFlags { get; set; }

    /// <summary>
    /// Gets the playlist id.
    /// </summary>
    public ushort PlaylistId => Identifier.Id;

    /// <summary>
    /// Builds the media info from the BluRay source.
    /// </summary>
    /// <returns></returns>
    private MediaInfo BuildMediaInfo()
    {
        // Build segments
        var playlistDuration = TimeSpan.Zero;
        var segmentInfos = new List<SegmentInfo>();
        foreach (var item in _playlist.Items)
        {
            if (!ushort.TryParse(item.Name, out var clipId))
                continue;
            
            // Video streams
            var first = true;
            var videoStreamInfos = new List<VideoInfo>();
            foreach (var stream in item.StreamNumberTable.PrimaryVideoStreams)
            {
                if (stream.Entry.RefToStreamId == 0) continue;
                var streamInfo = new VideoInfo
                {
                    Id = stream.Entry.RefToStreamId,
                    Name = GetDescriptionFromStream(stream),
                    IsDefault = first
                };
                videoStreamInfos.Add(streamInfo);
                first = false;
            }
            foreach (var stream in item.StreamNumberTable.SecondaryVideoStream)
            {
                if (stream.Entry.RefToStreamId == 0) continue;
                var streamInfo = new VideoInfo
                {
                    Id = stream.Entry.RefToStreamId,
                    Name = GetDescriptionFromStream(stream),
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
                var streamInfo = new AudioInfo
                {
                    Id = stream.Entry.RefToStreamId,
                    Name = GetDescriptionFromStream(stream),
                    LanguageCode = stream.Attributes.LanguageCode,
                    IsDefault = first
                };
                audioStreamInfos.Add(streamInfo);
                first = false;
            }
            foreach (var stream in item.StreamNumberTable.SecondaryAudioStream)
            {
                if (stream.Entry.RefToStreamId == 0) continue;
                var streamInfo = new AudioInfo
                {
                    Id = stream.Entry.RefToStreamId,
                    Name = GetDescriptionFromStream(stream),
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
                var streamInfo = new SubtitleInfo
                {
                    Id = stream.Entry.RefToStreamId,
                    Name = GetDescriptionFromStream(stream),
                    LanguageCode = stream.Attributes.LanguageCode,
                    IsDefault = first
                };
                subtitleStreamInfos.Add(streamInfo);
                first = false;
            }
            foreach (var stream in item.StreamNumberTable.SecondaryPgStream)
            {
                if (stream.Entry.RefToStreamId == 0) continue;
                var streamInfo = new SubtitleInfo
                {
                    Id = stream.Entry.RefToStreamId,
                    Name = GetDescriptionFromStream(stream),
                    LanguageCode = stream.Attributes.LanguageCode,
                    IsSecondary = true,
                    IsDefault = first
                };
                subtitleStreamInfos.Add(streamInfo);
                first = false;
            }
                
            var segmentInfo = new SegmentInfo
            {
                Id = clipId,
                Name = $"Segment {clipId}",
                Duration = BluRay.TimeSpanFromBluRayTime(item.Duration),
                VideoStreams = videoStreamInfos.ToArray(),
                AudioStreams = audioStreamInfos.ToArray(),
                SubtitleStreams = subtitleStreamInfos.ToArray(),
            };
            playlistDuration += segmentInfo.Duration;
            segmentInfos.Add(segmentInfo);
        }
        
        // Build chapters
        var chapterInfos = new List<ChapterInfo>();
        var chapterTimestamps = new List<TimeSpan>();
        foreach (var mark in _playlist.Marks)
        {
            // Adding all previous durations
            uint offset = 0;
            for (var i = 0; i < mark.PlayItemId; i++)
            {
                offset += _playlist.Items[i].Duration;
            }
            var item = _playlist.Items[mark.PlayItemId];
            var timestamp = BluRay.TimeSpanFromBluRayTime(offset + mark.TimeStamp - item.InTime);
            // Avoid negative
            if (timestamp < TimeSpan.Zero) timestamp = TimeSpan.Zero;
            // Avoid timestamp above max length. Also add three-second tolerance.
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
                Id = (ushort)chapterInfos.Count,
                Name = $"Chapter {chapterInfos.Count+1:00}",
                Start = start,
                End = end
            };
            chapterInfos.Add(chapterInfo);
        }
        
        return new MediaInfo
        {
            Id = PlaylistId,
            Name = $"Playlist {PlaylistId}",
            Duration = playlistDuration,
            Segments = segmentInfos.ToArray(),
            Chapters = chapterInfos.ToArray(),
        };
    }
    
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

    #endregion Media info
    
    #region Output
    
    /// <inheritdoc />
    public OutputDefinition CreateDefaultOutputDefinition(CodecOptions codec, VideoFormat format)
    {
        if (_playlist.Items.Length == 0)
            throw new ArgumentException("Cannot create output for title without segments!", nameof(PlaylistId));

        var baseName = $"{Identifier.DiskName}_{PlaylistId}";
        var segment = _playlist.Items[0];
        var exportSubtitlesAsSeparateFiles = !format.SupportPgs;
        
        // Calculate total duration
        var duration = TimeSpan.Zero;
        foreach (var item in _playlist.Items)
        {
            duration += BluRay.TimeSpanFromBluRayTime(item.Duration);
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
        foreach (var mark in _playlist.Marks)
        {
            // Adding all previous durations
            uint offset = 0;
            for (var i = 0; i < mark.PlayItemId; i++)
            {
                offset += _playlist.Items[i].Duration;
            }
            var item = _playlist.Items[mark.PlayItemId];
            var timestamp = BluRay.TimeSpanFromBluRayTime(offset + mark.TimeStamp - item.InTime);
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
    
    #region Equals

    /// <summary>
    /// Compares this playlist with another one and return true if it matches.
    /// </summary>
    /// <param name="other">The other playlist.</param>
    /// <returns></returns>
    public bool Matches(BluRayMediaSource other)
    {
        if (ReferenceEquals(null, other)) return false;
        
        // Compare segments
        if (Info.Segments.Length != other.Info.Segments.Length) return false;
        for (var i = 0; i < Info.Segments.Length; i++)
        {
            var segment = Info.Segments[i];
            var otherSegment = other.Info.Segments[i];
            
            if (segment.Id != otherSegment.Id) return false;
            
            // Compare video streams
            if (segment.VideoStreams.Length != otherSegment.VideoStreams.Length) return false;
            for (var j = 0; j < segment.VideoStreams.Length; j++)
            {
                var stream = segment.VideoStreams[j];
                var otherStream = otherSegment.VideoStreams[j];
                
                if (stream.Id != otherStream.Id) return false;
            }
            
            // Compare audio streams
            if (segment.AudioStreams.Length != otherSegment.AudioStreams.Length) return false;
            for (var j = 0; j < segment.AudioStreams.Length; j++)
            {
                var stream = segment.AudioStreams[j];
                var otherStream = otherSegment.AudioStreams[j];
                
                if (stream.Id != otherStream.Id) return false;
            }
            
            // Compare subtitle streams
            if (segment.SubtitleStreams.Length != otherSegment.SubtitleStreams.Length) return false;
            for (var j = 0; j < segment.SubtitleStreams.Length; j++)
            {
                var stream = segment.SubtitleStreams[j];
                var otherStream = otherSegment.SubtitleStreams[j];
                
                if (stream.Id != otherStream.Id) return false;
            }
        }
        
        // Compare chapters
        if (Info.Chapters.Length != other.Info.Chapters.Length) return false;
        for (var i = 0; i < Info.Chapters.Length; i++)
        {
            var chapter = Info.Chapters[i];
            var otherChapter = other.Info.Chapters[i];
            
            if (chapter.Start != otherChapter.Start) return false;
            if (chapter.End != otherChapter.End) return false;
        }
        
        return true;
    }
    
    #endregion Equals
}