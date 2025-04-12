namespace BluRayLib.Ripper.BluRays;

public class TitleData
{
    public TitleData(ushort id)
    {
        Id = id;
    }

    /// <summary>
    /// Gets the playlist id.
    /// </summary>
    public ushort Id { get; }
    
    /// <summary>
    /// Gets the playlist duration.
    /// </summary>
    public TimeSpan Duration { get; set; }
    
    /// <summary>
    /// Gets the segments / clips.
    /// </summary>
    public SegmentData[] Segments { get; set; } = [];

    /// <summary>
    /// Gets the chapters.
    /// </summary>
    public ChapterInfo[] Chapters { get; set; } = [];
    
    /// <summary>
    /// Gets the ignore flags.
    /// </summary>
    public TitleIgnoreFlags IgnoreFlags { get; set; }
    
    public override string ToString() => $"Playlist [0x{Id:x4}] ({Id:00000}) - [{Duration:hh\\:mm\\:ss}] ({Segments.Length} segments) ({IgnoreFlags})";
    
    #region Equals

    /// <summary>
    /// Compares this playlist with another one and return true if it matches.
    /// </summary>
    /// <param name="other">The other playlist.</param>
    /// <returns></returns>
    public bool Matches(TitleData? other)
    {
        if (ReferenceEquals(null, other)) return false;

        // Compare segments
        if (Segments.Length != other.Segments.Length) return false;
        for (var i = 0; i < Segments.Length; i++)
        {
            var segment = Segments[i];
            var otherSegment = other.Segments[i];
            
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
        if (Chapters.Length != other.Chapters.Length) return false;
        for (var i = 0; i < Chapters.Length; i++)
        {
            var chapter = Chapters[i];
            var otherChapter = other.Chapters[i];
            
            if (chapter.Start != otherChapter.Start) return false;
            if (chapter.End != otherChapter.End) return false;
        }
        
        return true;
    }
    
    #endregion Equals
}