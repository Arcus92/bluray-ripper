using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using MediaRipper.Services.Interfaces;
using MediaRipper.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace MediaRipper.Services;

public class ApplicationService : IApplicationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<ViewModelBase, Window> _viewModelsToWindow = new();

    public ApplicationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    /// <inheritdoc />
    public T ShowWindow<T>() where T : ViewModelBase
    {
        var viewModel = _serviceProvider.GetRequiredService<T>();
        var window = (Window)viewModel.CreateView();
        window.DataContext = viewModel;
        window.Show();
        _viewModelsToWindow.Add(viewModel, window);
        window.Closed += (_, _) => _viewModelsToWindow.Remove(viewModel);
        return viewModel;
    }

    /// <inheritdoc />
    public bool TryGetWindow(ViewModelBase viewModel, [MaybeNullWhen(false)] out Window window)
    {
        return _viewModelsToWindow.TryGetValue(viewModel, out window);
    }

    /// <inheritdoc />
    public void CloseWindow(ViewModelBase viewModel)
    {
        if (!_viewModelsToWindow.TryGetValue(viewModel, out var window))
        {
            return;
        }
        
        window.Close();
    }
}