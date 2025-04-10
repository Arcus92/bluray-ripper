using CommunityToolkit.Mvvm.ComponentModel;

namespace BluRayRipper.Models.Nodes;

public class BaseNode : ObservableObject
{
    /// <inheritdoc cref="IsExpanded"/>
    private bool _isExpanded;

    /// <summary>
    /// Gets and sets if this node is expanded.
    /// </summary>
    public bool IsExpanded
    {
        get => _isExpanded;
        set => SetProperty(ref _isExpanded, value);
    }
}