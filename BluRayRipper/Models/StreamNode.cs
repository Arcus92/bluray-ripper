using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BluRayRipper.Models;

public class StreamNode : ObservableObject
{
    public StreamNode(string name, ObservableCollection<StreamNode> subNodes)
    {
        Name = name;
        SubNodes = subNodes;
    }

    public StreamNode(string name, IEnumerable<StreamNode> subNodes) : this(name, new ObservableCollection<StreamNode>(subNodes))
    {
    }
    
    public StreamNode(string name) : this(name, [])
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
    public ObservableCollection<StreamNode> SubNodes { get; }
}