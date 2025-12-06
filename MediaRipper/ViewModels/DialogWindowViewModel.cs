using Avalonia.Controls;
using MediaRipper.Services.Interfaces;
using MediaRipper.Views;

namespace MediaRipper.ViewModels;

public class DialogWindowViewModel : ViewModelBase
{
    private readonly IApplicationService _applicationService;

    public DialogWindowViewModel(IApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    /// <summary>
    /// Gets and sets the message text.
    /// </summary>
    public string Text
    {
        get;
        set => SetProperty(ref field, value);
    } = "";

    /// <summary>
    /// Closes this dialog message.
    /// </summary>
    public void Close()
    {
        _applicationService.CloseWindow(this);
    }
    
    /// <inheritdoc/>
    public override Control CreateView()
    {
        return new DialogWindow();
    }
}