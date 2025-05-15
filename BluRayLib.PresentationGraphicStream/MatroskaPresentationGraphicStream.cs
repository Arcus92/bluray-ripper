using Matroska.Models;
using MediaLib.Utils.IO;

namespace BluRayLib.PresentationGraphicStream;

/// <summary>
/// A Presentation Graphic Stream (PGS) from a Matroska (MKV) subtitle track.
/// </summary>
public class MatroskaPresentationGraphicStream : IPresentationGraphicStream
{
    /// <summary>
    /// The source matroska document.
    /// </summary>
    private readonly MatroskaDocument _matroska;
    
    /// <summary>
    /// The track number of the PGS subtitle.
    /// </summary>
    private readonly ulong _trackNumber; 

    public MatroskaPresentationGraphicStream(MatroskaDocument matroska, ulong trackNumber)
    {
        _matroska = matroska;
        _trackNumber = trackNumber;
    }
    
    /// <inheritdoc />
    public async IAsyncEnumerable<DisplaySet> ReadAsync()
    {
        // Finds the source track
        var track = _matroska.Segment.Tracks?.TrackEntries.FirstOrDefault(t => t.TrackNumber == _trackNumber);
        if (track is null) throw new ArgumentException($"Track '{_trackNumber}' wasn't found.");
        
        var timestampScale = _matroska.Segment.Info.TimestampScale / 1_000_000.0;
        
        foreach (var (timestamp, block) in ReadBlocksForTrackNumber())
        {
            if (block.TrackNumber != _trackNumber) continue;
            if (block.Data is null) continue;

            var timeInMilliseconds =
                (timestamp + (double)block.TimeCode) * timestampScale - (track.CodecDelay ?? 0);

            await using var ms = new MemoryStream(block.Data);
            using var reader = new BigEndianBinaryReader(ms);
            var displaySet = new DisplaySet();

            var pgsTimestamp = (uint)(timeInMilliseconds * 90);
            displaySet.PresentationTimestamp = pgsTimestamp;
            // Decoding time isn't used and set to 0 most of the time. FFmpeg sets it to the presentation
            // timestamp, so I'll do it as well.
            displaySet.DecodingTimestamp = pgsTimestamp;
            displaySet.Read(reader, includeHeader: false);

            yield return displaySet;
        }
    }

    /// <summary>
    /// Returns all relevant blocks for the current track number.
    /// </summary>
    /// <returns>Returns the timestamps and block data.</returns>
    private IEnumerable<(ulong, Block)> ReadBlocksForTrackNumber()
    {
        if (_matroska.Segment.Clusters is null) yield break;
        foreach (var cluster in _matroska.Segment.Clusters)
        {
            if (cluster.BlockGroups is not null)
            {
                foreach (var blockGroup in cluster.BlockGroups)
                {
                    foreach (var block in blockGroup.Blocks)
                    {
                        if (block.TrackNumber != _trackNumber) continue;
                        if (block.Data is null) continue;

                        yield return (cluster.Timestamp, block);
                    }
                }
            }
            
            if (cluster.SimpleBlocks is not null)
            {
                foreach (var block in cluster.SimpleBlocks)
                {
                    if (block.TrackNumber != _trackNumber) continue;
                    if (block.Data is null) continue;
                    yield return (cluster.Timestamp, block);
                }
            }
        }
    }
}