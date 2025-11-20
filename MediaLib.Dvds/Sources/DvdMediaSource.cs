using DvdLib.Streams;
using MediaLib.Formats;
using MediaLib.Models;
using MediaLib.Output;
using MediaLib.Sources;

namespace MediaLib.Dvds.Sources;

public class DvdMediaSource : IMediaSource
{
    private readonly VideoStream _videoStream;
    
    public DvdMediaSource(VideoStream videoStream, MediaIdentifier identifier)
    {
        _videoStream = videoStream;
        Identifier = identifier;
        IgnoreFlags = MediaIgnoreFlags.None;
        Info = BuildMediaInfo();
    }
    
    #region Media info
    
    /// <inheritdoc />
    public MediaIdentifier Identifier { get; }
    
    /// <inheritdoc />
    public MediaInfo Info { get; }
    
    /// <inheritdoc />
    public MediaIgnoreFlags IgnoreFlags { get; }

    /// <summary>
    /// Builds the media info from the DVD source.
    /// </summary>
    /// <returns></returns>
    private MediaInfo BuildMediaInfo()
    {
        var ifo = _videoStream.Information;
        var baseName = _videoStream.Identifier.ToFilename();
        ushort streamId = 1;
        
        return new MediaInfo
        {
            Id = _videoStream.Identifier.TitleSet,
            Name = baseName,
            Duration = TimeSpan.Zero,
            Segments = [new SegmentInfo
            {
                Id = 0,
                Name = baseName,
                Duration = TimeSpan.Zero,
                VideoStreams = [new VideoInfo
                {
                    Id = streamId++,
                    Name = baseName,
                    IsDefault = true
                }],
                AudioStreams = ifo.AudioList.Select((a, n) => new AudioInfo()
                {
                    Id = streamId++,
                    Name = a.LangCode,
                    LanguageCode = a.LangCode,
                }).ToArray(),
                SubtitleStreams = ifo.SubPictureList.Select((s, n) => new SubtitleInfo()
                {
                    Id = streamId++,
                    Name = s.LangCode,
                    LanguageCode = s.LangCode
                }).ToArray()
            }],
            Chapters = [],
        };
    }
    
    #endregion Media info
    
    #region Output
    
    /// <inheritdoc />
    
    public OutputDefinition CreateDefaultOutputDefinition(CodecOptions codec, MediaFormat containerFormat)
    {
        var baseName = _videoStream.Identifier.ToFilename();
        var duration = TimeSpan.Zero;

        ushort streamId = 1;
        
        var streams = new List<OutputStream>();
        streams.Add(new OutputStream
        {
            Enabled = true,
            Id = streamId++,
            Type = OutputStreamType.Video
        });

        foreach (var audio in _videoStream.Information.AudioList)
        {
            streams.Add(new OutputStream
            {
                Enabled = true,
                Id = streamId++,
                LanguageCode = audio.LangCode,
                Type = OutputStreamType.Audio
            });
        }
        
        foreach (var subPicture in _videoStream.Information.SubPictureList)
        {
            streams.Add(new OutputStream
            {
                Enabled = true,
                Id = streamId++,
                LanguageCode = subPicture.LangCode,
                Type = OutputStreamType.Subtitle,
                Format = "dvd_subtitle"
            });
        }
        
        var files = OutputHelper.GetFilesByStreams(baseName, streams, codec, containerFormat);
        
        var chapterInfos = new List<OutputChapter>();
        
        
        return new OutputDefinition
        {
            Identifier = Identifier,
            MediaInfo = new OutputMediaInfo
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