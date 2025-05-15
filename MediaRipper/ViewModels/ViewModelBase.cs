using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MediaRipper.ViewModels;

/// <summary>
/// Base class for view models.
/// </summary>
public abstract class ViewModelBase : ObservableObject
{
    /// <summary>
    /// Creates the view object.
    /// </summary>
    /// <returns>The view object.</returns>
    public abstract Control CreateView(); 
}