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
    /// <returns>Returns the view model.</returns>
    T ShowWindow<T>() where T : ViewModelBase;
    
    /// <summary>
    /// Tries to find the window attached to the given view model.
    /// </summary>
    /// <param name="viewModel">The view model to find.</param>
    /// <param name="window">Returns the window.</param>
    /// <returns>Returns if the window of the view model was found.</returns>
    bool TryGetWindow(ViewModelBase viewModel, [MaybeNullWhen(false)] out Window window);

    /// <summary>
    /// Closes the window attached to the given view model.
    /// </summary>
    /// <param name="viewModel">The view model to close.</param>
    void CloseWindow(ViewModelBase viewModel);
}