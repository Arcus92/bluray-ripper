using Avalonia.Controls;
using Avalonia.Controls.Templates;
using BluRayRipper.ViewModels;

namespace BluRayRipper;

public class ViewLocator : IDataTemplate
{
    /// <inheritdoc />
    public Control? Build(object? param)
    {
        if (param is not ViewModelBase viewModelBase)
            return null;

        return viewModelBase.CreateView();
    }

    /// <inheritdoc />
    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}