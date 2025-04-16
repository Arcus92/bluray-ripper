using Avalonia.Controls;
using BluRayRipper.Views;

namespace BluRayRipper.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public DiskSelectorViewModel DiskSelector { get; }
    public TitleTreeViewModel TitleTree { get; }
    public TitleOptionsViewModel TitleOptions { get; }
    public OutputSelectorViewModel OutputSelector { get; }
    public OutputSettingsViewModel OutputSettings { get; }
    public OutputListViewModel OutputList { get; }
    
    public MainWindowViewModel(DiskSelectorViewModel diskSelector, TitleTreeViewModel titleTree, 
        TitleOptionsViewModel titleOptions, OutputSelectorViewModel outputSelector, 
        OutputSettingsViewModel outputSettings, OutputListViewModel outputList)
    {
        DiskSelector = diskSelector;
        TitleTree = titleTree;
        TitleOptions = titleOptions;
        OutputSelector = outputSelector;
        OutputSettings = outputSettings;
        OutputList = outputList;
    }

    /// <inheritdoc />
    public override Control CreateView() => new MainWindow();
}