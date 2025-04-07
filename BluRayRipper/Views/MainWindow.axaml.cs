using Avalonia.Controls;
using Avalonia.Interactivity;
using BluRayRipper.ViewModels;

namespace BluRayRipper.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    private async void OnLoadDiskClick(object? sender, RoutedEventArgs e)
    {
        var dataContext = (MainWindowViewModel)DataContext!;
        await dataContext.LoadDiskAsync();
    }
    
    private async void OnExportClick(object? sender, RoutedEventArgs e)
    {
        var dataContext = (MainWindowViewModel)DataContext!;
        await dataContext.ExportAsync();
    }
    
    private async void OnDecryptClick(object? sender, RoutedEventArgs e)
    {
        var dataContext = (MainWindowViewModel)DataContext!;
        await dataContext.DecryptAsync();
    }
}