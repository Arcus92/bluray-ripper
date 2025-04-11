using BluRayRipper.Services;
using BluRayRipper.Services.Interfaces;
using BluRayRipper.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace BluRayRipper;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the common services for this application.
    /// </summary>
    /// <param name="collection">The service collection.</param>
    public static void AddCommonServices(this IServiceCollection collection)
    {
        // View models
        collection.AddScoped<MainWindowViewModel>();
        collection.AddScoped<DiskSelectorViewModel>();
        collection.AddScoped<TitleTreeViewModel>();
        collection.AddScoped<TitlePropertiesViewModel>();
        collection.AddScoped<OutputSelectorViewModel>();
        collection.AddScoped<OutputSettingsViewModel>();
        collection.AddScoped<OutputViewModel>();
        
        // Controller
        collection.AddSingleton<ISettingService, SettingService>();
        collection.AddSingleton<IDiskService, DiskService>();
        collection.AddSingleton<IOutputService, OutputService>();
        collection.AddSingleton<IQueueService, QueueService>();

        // Services
    }
}