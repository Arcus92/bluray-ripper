using System;
using Avalonia;
using Microsoft.Extensions.DependencyInjection;

namespace MediaRipper.Utils;

/// <summary>
/// A resolver for dependency injected services. This lets you fetch a service from the designer.
/// </summary>
/// <param name="type">The service type to resolve.</param>
public class DesignResolve(Type type)
{
    public object ProvideValue()
    {
        var app = (App)Application.Current!;
        return app.ServiceProvider.GetRequiredService(type);
    }
}