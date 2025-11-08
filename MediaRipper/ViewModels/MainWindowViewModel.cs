using Avalonia.Controls;
using MediaRipper.Views;

namespace MediaRipper.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public SourceSelectorViewModel SourceSelector { get; }
    public SourceTreeViewModel SourceTree { get; }
    public ExportSettingsViewModel ExportSettings { get; }
    public OutputSelectorViewModel OutputSelector { get; }
    public QueueSettingsViewModel QueueSettings { get; }
    public OutputTreeViewModel OutputTree { get; }
    public OutputSettingsViewModel OutputSettings { get; }
    
    public MainWindowViewModel(SourceSelectorViewModel sourceSelector, SourceTreeViewModel sourceTree, 
        ExportSettingsViewModel exportSettings, OutputSelectorViewModel outputSelector, 
        QueueSettingsViewModel queueSettings, OutputTreeViewModel outputTree, OutputSettingsViewModel outputSettings)
    {
        SourceSelector = sourceSelector;
        SourceTree = sourceTree;
        ExportSettings = exportSettings;
        OutputSelector = outputSelector;
        QueueSettings = queueSettings;
        OutputTree = outputTree;
        OutputSettings = outputSettings;
    }

    /// <inheritdoc />
    public override Control CreateView() => new MainWindow();
}