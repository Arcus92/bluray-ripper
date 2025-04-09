using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using BluRayLib.FFmpeg;
using BluRayRipper.Services;
using BluRayRipper.Services.Interfaces;
using BluRayRipper.Views;

namespace BluRayRipper.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public DiskSelectorViewModel DiskSelector { get; }
    public TitleTreeViewModel TitleTree { get; }
    public TitlePropertiesViewModel TitleProperties { get; }
    public OutputViewModel Output { get; }
    
    public MainWindowViewModel(DiskSelectorViewModel diskSelector, TitleTreeViewModel titleTree, 
        TitlePropertiesViewModel titleProperties, OutputViewModel output)
    {
        DiskSelector = diskSelector;
        TitleTree = titleTree;
        TitleProperties = titleProperties;
        Output = output;
    }
    
    // Designer default
    public MainWindowViewModel() : this(new DiskSelectorViewModel(), new TitleTreeViewModel(), 
        new TitlePropertiesViewModel(), new OutputViewModel())
    {
    }

    /// <inheritdoc />
    public override Control CreateView() => new MainWindow();
}