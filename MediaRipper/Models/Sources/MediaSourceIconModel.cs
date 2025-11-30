using FluentIcons.Common;

namespace MediaRipper.Models.Sources;

public class MediaSourceIconModel : BaseSourceModel
{
    /// <summary>
    /// Gets the icon.
    /// </summary>
    public Icon Icon { get; }
    
    /// <summary>
    /// Gets the description text of this item used by the tool-tip.
    /// </summary>
    public string Description { get; }
    
    public MediaSourceIconModel(Icon icon, string description)
    {
        Icon = icon;
        Description = description;
    }
}