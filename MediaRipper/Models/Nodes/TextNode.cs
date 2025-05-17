using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MediaRipper.Models.Nodes;

public class TextNode : BaseNode
{
    public TextNode(string text)
    {
        Text = text;
    }

    /// <summary>
    /// Gets the text.
    /// </summary>
    public string Text { get; }
    
    /// <summary>
    /// Gets the sub-nodes.
    /// </summary>
    public ObservableCollection<BaseNode> SubNodes { get; init; } = [];
}

public class TextNode<TChild> : TextNode where TChild : BaseNode
{
    public TextNode(string text) : base(text)
    {
    }

    /// <summary>
    /// Gets the items in this node.
    /// </summary>
    public IEnumerable<TChild> Items => SubNodes.Cast<TChild>();
}