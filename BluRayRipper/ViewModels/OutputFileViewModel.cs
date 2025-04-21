using Avalonia.Controls;
using BluRayRipper.Models.Output;
using BluRayRipper.Views;

namespace BluRayRipper.ViewModels;

public class OutputFileViewModel : ViewModelBase
{
    /// <summary>
    /// Gets the output file.
    /// </summary>
    public OutputFileModel Model { get; }

    public OutputFileViewModel(OutputFileModel model)
    {
        Model = model;
    }

    /// <inheritdoc />
    public override Control CreateView()
    {
        return new OutputFileView();
    }
}