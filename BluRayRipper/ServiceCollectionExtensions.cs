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
        collection.AddTransient<MainWindowViewModel>();
        collection.AddTransient<DiskSelectorViewModel>();
        collection.AddTransient<TitleTreeViewModel>();
        collection.AddTransient<TitlePropertiesViewModel>();
        collection.AddTransient<OutputViewModel>();
        
        // Controller
        collection.AddSingleton<IDiskService, DiskService>();
        collection.AddSingleton<IQueueService, QueueService>();

        // Services
    }
}