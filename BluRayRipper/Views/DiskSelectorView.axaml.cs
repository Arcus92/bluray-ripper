using Avalonia.Controls;
using Avalonia.Interactivity;
using BluRayRipper.ViewModels;

namespace BluRayRipper.Views;

public partial class DiskSelectorView : UserControl
{
    public DiskSelectorView()
    {
        InitializeComponent();
    }

    private async void OnLoadClick(object? sender, RoutedEventArgs e)
    {
        var dataContext = (DiskSelectorViewModel)DataContext!;
        await dataContext.LoadDiskAsync();
    }
    
    private async void OnExportClick(object? sender, RoutedEventArgs e)
    {
        var dataContext = (DiskSelectorViewModel)DataContext!;
        await dataContext.QueueExportAsync();
    }
}