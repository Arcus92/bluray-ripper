using MediaLib.Models;

namespace MediaRipper.Models.Nodes;

public class StreamNode : BaseNode
{
    public StreamNode(StreamInfo stream)
    {
        Stream = stream;
    }
    
    /// <summary>
    /// Gets the stream info.
    /// </summary>
    public StreamInfo Stream { get; }
    
    /// <summary>
    /// Gets the stream id.
    /// </summary>
    public ushort Id => Stream.Id;
    
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