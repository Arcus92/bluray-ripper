using MediaRipper.Services;
using MediaRipper.Services.Interfaces;
using MediaRipper.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MediaRipper;

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
        collection.AddScoped<SourceSelectorViewModel>();
        collection.AddScoped<SourceTreeViewModel>();
        collection.AddScoped<ExportSettingsViewModel>();
        collection.AddScoped<OutputSelectorViewModel>();
        collection.AddScoped<QueueSettingsViewModel>();
        collection.AddScoped<OutputListViewModel>();
        collection.AddScoped<OutputTreeViewModel>();
        collection.AddScoped<OutputSettingsContainerViewModel>();
        
        // Controller
        collection.AddSingleton<ISettingService, SettingService>();
        collection.AddSingleton<IMediaProviderService, MediaProviderService>();
        collection.AddSingleton<IOutputService, OutputService>();
        collection.AddSingleton<IOutputQueueService, OutputQueueService>();

        // Services
        collection.AddLogging(builder => builder.AddConsole());
    }
}