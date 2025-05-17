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

            if (sourceA.Matches(sourceB))
            {
                sourceB.IgnoreFlags |= MediaIgnoreFlags.Duplicate;
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
        
        // Builds the media identifier
        var identifier = new MediaIdentifier()
        {
            Type = MediaIdentifierType.BluRay,
            ContentHash = BluRay.ContentHash,
            DiskName = BluRay.DiskName,
            Id = playlistId,
            SegmentIds = playlist.Items.Select(i => ushort.Parse(i.Name)).ToArray(),
        };
        
        var source = new BluRayMediaSource(playlist, identifier);

        // Check ignore flags
        var flags = MediaIgnoreFlags.None;

        // Smaller than 10 seconds
        if (source.Info.Duration.TotalSeconds < 10) 
        {
            flags |= MediaIgnoreFlags.TooShort;
        }
        
        // Longer than 5 hours
        if (source.Info.Duration.TotalSeconds > 60 * 60 * 5) 
        {
            flags |= MediaIgnoreFlags.TooLong;
        }

        // Scan segments
        var audioStreams = 0;
        var subtitleStreams = 0;
        foreach (var segment in source.Info.Segments)
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
        
        source.IgnoreFlags = flags;
        
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

    #region Dispose
    
    /// <inheritdoc />
    public void Dispose()
    {
        // Nothing to do
    }
    
    #endregion Dispose
}