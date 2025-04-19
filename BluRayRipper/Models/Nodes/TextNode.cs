using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BluRayRipper.Models.Nodes;

public class TextNode : BaseNode
{
    public TextNode(string displayName)
    {
        DisplayName = displayName;
    }

    /// <summary>
    /// Gets the text resource key.
    /// </summary>
    public string DisplayName { get; }
    
    /// <summary>
    /// Gets the sub-nodes.
    /// </summary>
    public ObservableCollection<BaseNode> SubNodes { get; init; } = [];
}

public class TextNode<TChild> : TextNode where TChild : BaseNode
{
    public TextNode(string displayName) : base(displayName)
    {
    }

    /// <summary>
    /// Gets the items in this node.
    /// </summary>
    public IEnumerable<TChild> Items => SubNodes.Cast<TChild>();
}