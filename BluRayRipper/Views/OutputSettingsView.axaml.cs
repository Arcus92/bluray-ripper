using Avalonia.Controls;
using Avalonia.Interactivity;
using BluRayRipper.ViewModels;

namespace BluRayRipper.Views;

public partial class OutputSettingsView : UserControl
{
    public OutputSettingsView()
    {
        InitializeComponent();
    }
    
    private async void OnExportClick(object? sender, RoutedEventArgs e)
    {
        var dataContext = (OutputSettingsViewModel)DataContext!;
        await dataContext.QueueExportAsync();
    }
    
    private async void OnPreviewClick(object? sender, RoutedEventArgs e)
    {
        var dataContext = (OutputSettingsViewModel)DataContext!;
        await dataContext.PlayPreviewAsync();
    }
}