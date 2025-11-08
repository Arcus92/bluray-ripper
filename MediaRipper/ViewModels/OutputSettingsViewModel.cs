using System.ComponentModel;
using Avalonia.Controls;
using MediaRipper.Services.Interfaces;
using MediaRipper.Views;

namespace MediaRipper.ViewModels;

public class OutputSettingsViewModel : ViewModelBase
{
    private readonly IOutputService _outputService;
    private readonly OutputTreeViewModel _outputTreeViewModel;
    
    public OutputSettingsViewModel(IOutputService outputService, OutputTreeViewModel outputTreeViewModel)
    {
        _outputService = outputService;
        _outputTreeViewModel =  outputTreeViewModel;
        _outputTreeViewModel.PropertyChanged += OnOutputTreeViewModelPropertyChanged;
    }

    private void OnOutputTreeViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(OutputTreeViewModel.SelectedItem):
                var model = _outputTreeViewModel.SelectedItem?.Model;
                SelectedItem = model is null ? null : new OutputViewModel(_outputService, model);
                break;
        }
    }

    /// <inheritdoc cref="SelectedItem"/>
    private OutputViewModel? _selectedItem;
    
    /// <summary>
    /// Gets and sets the selected output.
    /// </summary>
    public OutputViewModel? SelectedItem
    {
        get => _selectedItem;
        set => SetProperty(ref _selectedItem, value);
    }
    
    /// <inheritdoc />
    public override Control CreateView()
    {
        return new OutputSettingsView();
    }
}