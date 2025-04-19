using Avalonia.Controls;
using BluRayRipper.Models.Output;
using BluRayRipper.Views;

namespace BluRayRipper.ViewModels;

public class OutputFileViewModel : ViewModelBase
{
    private readonly OutputFileModel _model;

    public OutputFileViewModel(OutputFileModel model)
    {
        _model = model;
    }
    
    /// <inheritdoc cref="OutputFileModel.Filename"/>
    public string Filename => _model.Filename;
    
    /// <inheritdoc cref="OutputFileModel.Size"/>
    public long Size => _model.Size;

    /// <inheritdoc />
    public override Control CreateView()
    {
        return new OutputFileView();
    }
}