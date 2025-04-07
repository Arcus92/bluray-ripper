using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BluRayRipper.ViewModels;

public abstract class ViewModelBase : ObservableObject
{
    public abstract Control CreateView(); 
}