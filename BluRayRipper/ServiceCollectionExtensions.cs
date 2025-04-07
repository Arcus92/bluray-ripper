using BluRayRipper.Controller;
using BluRayRipper.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace BluRayRipper;

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection collection)
    {
        collection.AddTransient<MainWindowViewModel>();
        collection.AddTransient<BluRayController>();
    }
}