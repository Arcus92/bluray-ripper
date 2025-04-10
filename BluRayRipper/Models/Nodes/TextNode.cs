using System.Collections.ObjectModel;

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
    public ObservableCollection<BaseNode> SubNodes { get; set; } = [];
}