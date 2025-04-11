using Avalonia.Controls;
using BluRayRipper.Views;

namespace BluRayRipper.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public DiskSelectorViewModel DiskSelector { get; }
    public TitleTreeViewModel TitleTree { get; }
    public TitlePropertiesViewModel TitleProperties { get; }
    public OutputSelectorViewModel OutputSelector { get; }
    public OutputSettingsViewModel OutputSettings { get; }
    public OutputViewModel Output { get; }
    
    public MainWindowViewModel(DiskSelectorViewModel diskSelector, TitleTreeViewModel titleTree, 
        TitlePropertiesViewModel titleProperties, OutputSelectorViewModel outputSelector, 
        OutputSettingsViewModel outputSettings, OutputViewModel output)
    {
        DiskSelector = diskSelector;
        TitleTree = titleTree;
        TitleProperties = titleProperties;
        OutputSelector = outputSelector;
        OutputSettings = outputSettings;
        Output = output;
    }
    
    // Designer default
    public MainWindowViewModel() : this(new DiskSelectorViewModel(), new TitleTreeViewModel(), 
        new TitlePropertiesViewModel(), new OutputSelectorViewModel(), new OutputSettingsViewModel(), 
        new OutputViewModel())
    {
    }

    /// <inheritdoc />
    public override Control CreateView() => new MainWindow();
}