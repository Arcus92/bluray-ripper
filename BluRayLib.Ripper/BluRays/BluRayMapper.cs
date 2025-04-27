using BluRayLib.Ripper.Output;

namespace BluRayLib.Ripper.BluRays;

public static class BluRayMapper
{
    /// <summary>
    /// Creates an output info object from the given BluRay title with the given codec and format.
    /// </summary>
    /// <param name="title">The BluRay title.</param>
    /// <param name="codec">The codec options.</param>
    /// <param name="format">The video format.</param>
    /// <param name="diskInfo">The disk info.</param>
    /// <returns></returns>
    public static OutputInfo ToOutputInfo(this TitleData title, CodecOptions codec, VideoFormat format, DiskInfo diskInfo)
    {
        if (title.Segments.Length == 0)
            throw new ArgumentException("Cannot create output for title without segments!", nameof(title));

        var baseName = $"{diskInfo.DiskName}_{title.Id}";
        var segment = title.Segments[0];
        var exportSubtitlesAsSeparateFiles = !format.SupportPgs;
        
        // Builds the BluRay source
        var source = new OutputSource()
        {
            Type = OutputSourceType.BluRay,
            DiskName = diskInfo.DiskName,
            ContentHash = diskInfo.ContentHash,
            PlaylistId = title.Id,
            Segments = title.Segments.Select(s => new OutputSegment()
            {
                Id = s.Id
            }).ToArray(),
        };
        
        // Collect all streams
        var streams = new List<OutputStream>();
        foreach (var stream in segment.VideoStreams)
        {
            streams.Add(new OutputStream()
            {
                Id = stream.Id,
                Type = OutputStreamType.Video,
                Default = stream.IsDefault,
            });
        }
        foreach (var stream in segment.AudioStreams)
        {
            streams.Add(new OutputStream()
            {
                Id = stream.Id,
                Type = OutputStreamType.Audio,
                LanguageCode = stream.LanguageCode,
                Default = stream.IsDefault,
            });
        }
        foreach (var stream in segment.SubtitleStreams)
        {
            streams.Add(new OutputStream()
            {
                Id = stream.Id,
                Type = OutputStreamType.Subtitle,
                LanguageCode = stream.LanguageCode,
                Default = stream.IsDefault,
            });
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
            foreach (var stream in streams.Where(s => s.Type is OutputStreamType.Subtitle))
            {
                files.Add(new OutputFile()
                {
                    Filename = $"{baseName}.{stream.LanguageCode}.{stream.Id}.sup",
                    Format = "sup",
                    Streams = [stream]
                });
            }
        }
        
        
        return new OutputInfo()
        {
            Source = source,
            MediaInfo = new OutputMediaInfo()
            {
                Name = baseName,
            },
            Duration = title.Duration,
            Codec = codec,
            Files = files.ToArray(),
            Chapters = title.Chapters.Select(c => new OutputChapter()
            {
                Name = c.Name,
                Start = c.Start,
                End = c.End,
            }).ToArray()
        };
    }
}