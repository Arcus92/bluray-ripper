using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MediaRipper.Models.Sources;

public class TextSourceModel : BaseSourceModel
{
    public TextSourceModel(string text)
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
    public ObservableCollection<BaseSourceModel> SubNodes { get; init; } = [];
}

public class TextSourceModel<TChild> : TextSourceModel where TChild : BaseSourceModel
{
    public TextSourceModel(string text) : base(text)
    {
    }

    /// <summary>
    /// Gets the items in this node.
    /// </summary>
    public IEnumerable<TChild> Items => SubNodes.Cast<TChild>();
}