using CommunityToolkit.Mvvm.ComponentModel;

namespace MediaRipper.Models.Sources;

public class BaseSourceModel : ObservableObject
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