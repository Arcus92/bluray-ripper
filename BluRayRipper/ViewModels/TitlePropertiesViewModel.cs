using Avalonia.Controls;
using BluRayRipper.Views;

namespace BluRayRipper.ViewModels;

public class TitlePropertiesViewModel : ViewModelBase
{
    /// <inheritdoc />
    public override Control CreateView()
    {
        return new TitlePropertiesView();
    }
}