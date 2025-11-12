using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using MediaRipper.Services.Interfaces;
using MediaRipper.ViewModels;
using MediaRipper.Views;
using Microsoft.Extensions.DependencyInjection;

namespace MediaRipper;

public partial class App : Application
{
    /// <summary>
    /// Gets the service provider for dependency injection.
    /// </summary>
    public ServiceProvider ServiceProvider { get; private set; } = null!;
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    public override void OnFrameworkInitializationCompleted()
    {
        // Register all services for this application
        var collection = new ServiceCollection();
        collection.AddCommonServices();
        
        ServiceProvider = collection.BuildServiceProvider();
        
        var mainViewModel = ServiceProvider.GetRequiredService<MainWindowViewModel>();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainViewModel,
            };
            
            var storageProviderAccessor = ServiceProvider.GetRequiredService<IStorageProviderAccessor>();
            storageProviderAccessor.StorageProvider = desktop.MainWindow.StorageProvider;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}