using System;
using BluRayLib.Ripper;

namespace BluRayRipper.Models.Output;

/// <summary>
/// Contains the information about a title export. This allows the application to remember a previous export.
/// </summary>
[Serializable]
public class OutputFile
{
    /// <summary>
    /// Gets and sets the disk name from which this output file was generated.
    /// </summary>
    public string DiskName { get; set; } = "";

    /// <summary>
    /// Gets and sets the main output filename.
    /// </summary>
    public string BaseName { get; set; } = "";
    
    /// <summary>
    /// Gets and sets the main output file extension.
    /// </summary>
    public string Extension { get; set; } = "";
    
    /// <summary>
    /// Gets and sets the output status.
    /// </summary>
    public OutputFileStatus Status { get; set; }
    
    /// <summary>
    /// Gets and sets the playlist id from which this output file was generated.
    /// </summary>
    public ushort PlaylistId { get; set; }

    /// <summary>
    /// Gets and sets the segment ids.
    /// </summary>
    public ushort[] SegmentIds { get; set; } = [];
    
    /// <summary>
    /// Gets and sets the codec options.
    /// </summary>
    public CodecOptions Codec { get; set; } = new();
    
    /// <summary>
    /// Gets and sets the exported video streams.
    /// </summary>
    public OutputStream[] VideoStreams { get; set; } = [];
    
    /// <summary>
    /// Gets and sets the exported audio streams.
    /// </summary>
    public OutputStream[] AudioStreams { get; set; } = [];
    
    /// <summary>
    /// Gets and sets the exported subtitle streams.
    /// </summary>
    public OutputStream[] SubtitleStreams { get; set; } = [];
}