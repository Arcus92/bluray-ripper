using BluRayLib.Ripper.BluRays;

namespace BluRayRipper.Models.Nodes;

public class StreamNode : BaseNode
{
    public StreamNode(StreamData stream)
    {
        Stream = stream;
    }
    
    /// <summary>
    /// Gets the stream info.
    /// </summary>
    public StreamData Stream { get; }
    
    /// <summary>
    /// Gets the stream id.
    /// </summary>
    public ushort Id => Stream.Id;
    
    /// <summary>
    /// Gets the title display name.
    /// </summary>
    public string DisplayName => Stream.ToString() ?? "";
    
    /// <inheritdoc cref="IsChecked"/>
    private bool _isChecked;
    
    /// <summary>
    /// Gets and sets if this stream is selected for export.
    /// </summary>
    public bool IsChecked
    {
        get => _isChecked;
        set => SetProperty(ref _isChecked, value);
    }
    
    /// <inheritdoc cref="IsDefault"/>
    private bool _isDefault;
    
    /// <summary>
    /// Gets and sets if this stream is the default track for the export.
    /// </summary>
    public bool IsDefault
    {
        get => _isDefault;
        set => SetProperty(ref _isDefault, value);
    }
}