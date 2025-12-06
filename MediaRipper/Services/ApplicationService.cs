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
    public Window ShowWindow<T>() where T : ViewModelBase
    {
        var viewModel = _serviceProvider.GetRequiredService<T>();
        return ShowWindow(viewModel);
    }
    
    /// <inheritdoc />
    public void ShowDialog<T>(T viewModel, ViewModelBase? owner = null) where T : ViewModelBase
    {
        Window? ownerWindow;
        if (owner is null)
        {
            if (!TryGetWindow<MainWindowViewModel>(out ownerWindow))
                return;
        }
        else
        {
            if (!TryGetWindow(owner, out ownerWindow))
                return;
        }
        
        var window = (Window)viewModel.CreateView();
        window.DataContext = viewModel;
        window.ShowDialog(ownerWindow);
        _viewModelsToWindow.Add(viewModel, window);
        window.Closed += (_, _) => _viewModelsToWindow.Remove(viewModel);
    }
    
    /// <inheritdoc />
    public void ShowDialog<T>(ViewModelBase? owner = null) where T : ViewModelBase
    {
        var viewModel = _serviceProvider.GetRequiredService<T>();
        ShowDialog(viewModel, owner);
    }
    
    /// <inheritdoc />
    public Window ShowWindow<T>(T viewModel) where T : ViewModelBase
    {
        var window = (Window)viewModel.CreateView();
        window.DataContext = viewModel;
        window.Show();
        _viewModelsToWindow.Add(viewModel, window);
        window.Closed += (_, _) => _viewModelsToWindow.Remove(viewModel);
        return window;
    }

    /// <inheritdoc />
    public bool TryGetWindow(ViewModelBase viewModel, [MaybeNullWhen(false)] out Window window)
    {
        return _viewModelsToWindow.TryGetValue(viewModel, out window);
    }

    /// <inheritdoc />
    public bool TryGetWindow<T>([MaybeNullWhen(false)] out Window window) where T : ViewModelBase => TryGetWindow<T>(out window, out _);
    
    /// <inheritdoc />
    public bool TryGetWindow<T>([MaybeNullWhen(false)] out Window window, [MaybeNullWhen(false)] out T viewModel)
        where T : ViewModelBase
    {
        foreach (var (curViewModel, curWindow) in _viewModelsToWindow)
        {
            if (curViewModel is not T curViewModelCasted) continue;
            
            window = curWindow;
            viewModel = curViewModelCasted;
            return true;
        }

        window = null;
        viewModel = null;
        return false;
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