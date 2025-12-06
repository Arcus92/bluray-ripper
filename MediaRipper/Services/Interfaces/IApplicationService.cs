using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using MediaRipper.ViewModels;

namespace MediaRipper.Services.Interfaces;

public interface IApplicationService
{
    /// <summary>
    /// Shows the window for the given view model.
    /// </summary>
    /// <typeparam name="T">The view model type to show.</typeparam>
    /// <returns>Returns the window.</returns>
    Window ShowWindow<T>() where T : ViewModelBase;
    
    /// <summary>
    /// Shows the window for the given view model.
    /// </summary>
    /// <param name="viewModel">The created view model.</param>
    /// <typeparam name="T">The view model type to show.</typeparam>
    /// <returns>Returns the window.</returns>
    Window ShowWindow<T>(T viewModel) where T : ViewModelBase;
    
    /// <summary>
    /// Shows the window for the given view model as dialog.
    /// </summary>
    /// <param name="owner">The parent window to block. The main application window is used if this is set to null.</param>
    /// <typeparam name="T">The view model type to show.</typeparam>
    void ShowDialog<T>(ViewModelBase? owner = null) where T : ViewModelBase;
    
    /// <summary>
    /// Shows the window for the given view model as dialog.
    /// </summary>
    /// <param name="viewModel">The created view model.</param>
    /// <param name="owner">The parent window to block. The main application window is used if this is set to null.</param>
    /// <typeparam name="T">The view model type to show.</typeparam>
    void ShowDialog<T>(T viewModel, ViewModelBase? owner = null) where T : ViewModelBase;
    
    /// <summary>
    /// Tries to find the window attached to the given view model.
    /// </summary>
    /// <param name="viewModel">The view model to find.</param>
    /// <param name="window">Returns the window.</param>
    /// <returns>Returns if the window of the view model was found.</returns>
    bool TryGetWindow(ViewModelBase viewModel, [MaybeNullWhen(false)] out Window window);

    /// <summary>
    /// Tries to find the window attached to the given view model type.
    /// </summary>
    /// <param name="window">Returns the window.</param>
    /// <typeparam name="T">The view model type to find.</typeparam>
    /// <returns>Returns if the window of the view model was found.</returns>
    bool TryGetWindow<T>([MaybeNullWhen(false)] out Window window) where T : ViewModelBase;
    
    /// <summary>
    /// Tries to find the window attached to the given view model type.
    /// </summary>
    /// <param name="window">Returns the window.</param>
    /// <param name="viewModel">Returns the view model.</param>
    /// <typeparam name="T">The view model type to find.</typeparam>
    /// <returns>Returns if the window of the view model was found.</returns>
    bool TryGetWindow<T>([MaybeNullWhen(false)] out Window window, [MaybeNullWhen(false)] out T viewModel) where T : ViewModelBase;
    
    /// <summary>e view model type to sh
    /// Closes the window attached to the given view model.
    /// </summary>
    /// <param name="viewModel">The view model to close.</param>
    void CloseWindow(ViewModelBase viewModel);
}