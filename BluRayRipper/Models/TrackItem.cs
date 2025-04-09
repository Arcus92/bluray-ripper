using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BluRayRipper.Models;

public class TrackItem : ObservableObject
{
    public TrackItem(string name, ObservableCollection<TrackItem> subNodes)
    {
        Name = name;
        SubNodes = subNodes;
    }

    public TrackItem(string name, IEnumerable<TrackItem> subNodes) : this(name, new ObservableCollection<TrackItem>(subNodes))
    {
    }
    
    public TrackItem(string name) : this(name, [])
    {
    }

    /// <summary>
    /// Gets the node name.
    /// </summary>
    public string Name { get; }

    private bool _isChecked;
    public bool IsChecked
    {
        get => _isChecked;
        set => SetProperty(ref _isChecked, value);
    }

    /// <summary>
    /// Gets the sub nodes.
    /// </summary>
    public ObservableCollection<TrackItem> SubNodes { get; }
}