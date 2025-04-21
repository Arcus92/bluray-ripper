using System.ComponentModel;
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
        _model.PropertyChanged += OnModelPropertyChanged;
    }

    private void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(OutputFileModel.Filename):
                OnPropertyChanged(nameof(Filename));
                break;
            case nameof(OutputFileModel.Size):
                OnPropertyChanged(nameof(Size));
                break;
        }
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