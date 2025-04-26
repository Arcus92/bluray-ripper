using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using BluRayLib.Ripper.Output;
using BluRayRipper.Models;
using BluRayRipper.Models.Output;
using BluRayRipper.Services.Interfaces;
using BluRayRipper.Views;

namespace BluRayRipper.ViewModels;

public class OutputViewModel : ViewModelBase
{
    private readonly IOutputService _outputService;
    
    /// <summary>
    /// Gets the title output instance.
    /// </summary>
    public OutputModel Model { get; }
    
    public OutputViewModel(IOutputService outputService, OutputModel model)
    {
        _outputService = outputService;
        
        Model = model;
        Model.PropertyChanged += ModelOnPropertyChanged;
        
        // Build externals
        Files = new ObservableCollection<OutputFileViewModel>(model.Files.Select(fileModel => new OutputFileViewModel(fileModel)));
    }
    
    private void ModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(OutputModel.Status):
                OnPropertyChanged(nameof(IsProgressBarVisible));
                break;
            case nameof(OutputModel.Progress):
                OnPropertyChanged(nameof(Progress));
                break;
        }
    }
    public ObservableCollection<OutputFileViewModel> Files { get; }
    
    #region Progress
    
    public bool IsProgressBarVisible => Model.Status == OutputStatus.Running;
    public double Progress => Model.Progress;
    
    #endregion Progress
    
    #region Metadata
    
    public static EnumModelList<OutputMediaType> AllMediaTypes { get; } =
    [
        new(OutputMediaType.Unset, "MediaTypeUnset"),
        new(OutputMediaType.Movie, "MediaTypeMovie"),
        new(OutputMediaType.Episode, "MediaTypeEpisode"),
        new(OutputMediaType.Extra, "MediaTypeExtra"),
        new(OutputMediaType.MakingOf, "MediaTypeMakingOf"),
        new(OutputMediaType.BehindTheScenes, "MediaTypeBehindTheScenes"),
        new(OutputMediaType.Interview, "MediaTypeInterview"),
        new(OutputMediaType.Trailer, "MediaTypeTrailer"),
    ];

    public EnumModel<OutputMediaType> MediaType
    {
        get => AllMediaTypes.GetModel(Model.Info.MediaInfo.Type);
        set => SetProperty(Model.Info.MediaInfo.Type, value.Value, v => Model.Info.MediaInfo.Type = v);
    }

    public string Name
    {
        get => Model.Info.MediaInfo.Name;
        set => SetProperty(Model.Info.MediaInfo.Name, value, v => Model.Info.MediaInfo.Name = v);
    }
    
    public int? Episode
    {
        get => Model.Info.MediaInfo.Episode;
        set => SetProperty(Model.Info.MediaInfo.Episode, value, v => Model.Info.MediaInfo.Episode = v);
    }
    
    public int? Season
    {
        get => Model.Info.MediaInfo.Season;
        set => SetProperty(Model.Info.MediaInfo.Season, value, v => Model.Info.MediaInfo.Season = v);
    }
    
    #endregion Metadata
    
    #region Commands
    
    public async Task ApplyAsync()
    {
        await _outputService.UpdateAsync(Model);
    }
    
    #endregion Commands
    
    /// <inheritdoc />
    public override Control CreateView()
    {
        return new OutputView();
    }
}