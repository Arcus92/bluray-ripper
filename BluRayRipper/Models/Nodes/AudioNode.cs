using BluRayLib.Ripper.BluRays;

namespace BluRayRipper.Models.Nodes;

public class AudioNode : BaseNode
{
    public AudioNode(AudioInfo stream)
    {
        Stream = stream;
    }

    /// <summary>
    /// Gets the stream info.
    /// </summary>
    public AudioInfo Stream { get; }
    
    /// <summary>
    /// Gets the stream id.
    /// </summary>
    public ushort Id => Stream.Id;
    
    /// <summary>
    /// Gets the title display name.
    /// </summary>
    public string DisplayName => Stream.ToString();
    
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
}