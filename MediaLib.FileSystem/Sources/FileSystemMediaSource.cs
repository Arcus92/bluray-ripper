using MediaLib.FFmpeg;
using MediaLib.Formats;
using MediaLib.Models;
using MediaLib.Output;
using MediaLib.Sources;

namespace MediaLib.FileSystem.Sources;

/// <summary>
/// A media source implementation for local files. 
/// </summary>
public class FileSystemMediaSource : IMediaSource
{
    private readonly string _name;
    private readonly string _path;
    private readonly InputMetadata _metadata;
    
    public FileSystemMediaSource(string path, MediaIdentifier identifier, InputMetadata metadata)
    {
        _path = path;
        _name = Path.GetFileNameWithoutExtension(path);
        _metadata = metadata;
        Identifier = identifier;
        Info = GetInfo(_name, metadata);
        IgnoreFlags = MediaIgnoreFlags.None;
    }

    /// <inheritdoc />
    public MediaIdentifier Identifier { get; }
    
    /// <inheritdoc />
    public MediaInfo Info { get; }
    
    /// <inheritdoc />
    public MediaIgnoreFlags IgnoreFlags { get; set; }
    
    /// <inheritdoc />
    public OutputDefinition CreateDefaultOutputDefinition(CodecOptions codec, MediaFormat containerFormat)
    {
        var baseName = _name;
        
        var streams = _metadata.Streams.Select(stream => new OutputStream()
        {
            Id = (ushort)stream.Id,
            Type = MapStreamType(stream.Type),
            Format = MapFormat(stream.Format),
            Default = stream.IsDefault,
            Enabled = true,
            LanguageCode = stream.Language
        });
        
        var files = OutputHelper.GetFilesByStreams(baseName, streams, codec, containerFormat);
        return new OutputDefinition
        {
            Identifier = Identifier,
            Codec = codec,
            Duration = _metadata.Duration,
            MediaInfo = new OutputMediaInfo
            {
                Name = _name,
            },
            Chapters = Info.Chapters.Select(chapter => new OutputChapter
            {
                Name = chapter.Name,
                Start = chapter.Start,
                End = chapter.End
            }).ToArray(),
            ExportChapters = true,
            Files = files.ToArray()
        };
    }

    /// <summary>
    /// Maps the FFmpeg stream type to the output stream type.
    /// </summary>
    /// <param name="streamType"></param>
    /// <returns></returns>
    private static OutputStreamType MapStreamType(StreamType streamType)
    {
        return streamType switch
        {
            StreamType.Video => OutputStreamType.Video,
            StreamType.Audio => OutputStreamType.Audio,
            StreamType.Subtitle => OutputStreamType.Subtitle,
            _ => throw new ArgumentOutOfRangeException(nameof(streamType), streamType, null)
        };
    }

    /// <summary>
    /// Maps the format to an FFmpeg format name. 
    /// </summary>
    /// <param name="format">The input format name.</param>
    /// <returns>Returns the FFmpeg format name.</returns>
    private static string MapFormat(string format)
    {
        return format switch
        {
            "subrip" => SubtitleFormats.Subrip.FFmpegFormat,
            _ => format
        };
    }

    /// <summary>
    /// Gets the media info from the given file.
    /// </summary>
    /// <param name="name">The name to the source file.</param>
    /// <param name="metadata">The FFmpeg metadata.</param>
    /// <returns>Returns the media info.</returns>
    public static MediaInfo GetInfo(string name, InputMetadata metadata)
    {
        return new MediaInfo
        {
            Id = 0,
            Name = name,
            Chapters = metadata.Chapters.Select((chapter, index) => new ChapterInfo()
            {
                Id = (ushort)index,
                Name = chapter.Title ?? $"Chapter {index + 1:00}",
                Start = chapter.Start,
                End = chapter.End,
            }).ToArray(),
            Duration = metadata.Duration,
            Segments = [new SegmentInfo
            {
                Id = 0,
                Name = name,
                Duration = metadata.Duration,
                VideoStreams = metadata.Streams
                    .Where(stream => stream.Type == StreamType.Video)
                    .Select(stream => new VideoInfo
                {
                    Id = stream.Pid,
                    Name = stream.Format,
                    IsDefault = stream.IsDefault
                }).ToArray(),
                AudioStreams = metadata.Streams
                    .Where(stream => stream.Type == StreamType.Audio)
                    .Select(stream => new AudioInfo
                    {
                        Id = stream.Pid,
                        Name = stream.Format,
                        LanguageCode = stream.Language,
                        IsDefault = stream.IsDefault
                    }).ToArray(),
                
                SubtitleStreams = metadata.Streams
                    .Where(stream => stream.Type == StreamType.Subtitle)
                    .Select(stream => new SubtitleInfo
                    {
                        Id = stream.Pid,
                        Name = stream.Format,
                        LanguageCode = stream.Language,
                        IsDefault = stream.IsDefault
                    }).ToArray(),
            }]
        };
    }
}