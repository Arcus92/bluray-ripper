using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using MediaLib.Output;
using MediaRipper.Models.MediaLookup;
using MediaRipper.Services.Interfaces;
using MediaRipper.Views;
using Microsoft.Extensions.Logging;

namespace MediaRipper.ViewModels;

public class MediaLookupViewModel : ViewModelBase
{
    private readonly ILogger<MediaLookupViewModel> _logger;
    private readonly IMediaLookupService _mediaLookupService;

    public MediaLookupViewModel(ILogger<MediaLookupViewModel> logger, IMediaLookupService mediaLookupService)
    {
        _logger = logger;
        _mediaLookupService = mediaLookupService;
    }

    /// <inheritdoc cref="SearchText" />
    private string _searchText = "";

    /// <summary>
    /// Gets and sets the search term.
    /// </summary>
    public string SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }
    
    /// <summary>
    /// Gets the search result list.
    /// </summary>
    public ObservableCollection<MediaSearchResult> SearchResults { get; } = [];

    /// <summary>
    /// Gets the season list.
    /// </summary>
    public ObservableCollection<MediaSeason> Seasons { get; } = [];
    
    /// <summary>
    /// Gets the episode list.
    /// </summary>
    public ObservableCollection<MediaEpisode> Episodes { get; } = [];
    
    /// <inheritdoc cref="IsLoading" />
    private bool _isLoading;

    /// <summary>
    /// Gets if the media lookup is loading.
    /// </summary>
    public bool IsLoading
    {
        get => _isLoading;
        private set => SetProperty(ref _isLoading, value);
    }
    
    /// <inheritdoc cref="SelectedMediaItem" />
    private MediaSearchResult? _selectedMediaItem;

    /// <summary>
    /// Gets and sets the selected search result.
    /// </summary>
    public MediaSearchResult? SelectedMediaItem
    {
        get => _selectedMediaItem;
        set => SetProperty(ref _selectedMediaItem, value);
    }
    
    /// <inheritdoc cref="SelectedSeason" />
    private MediaSeason? _selectedSeason;

    /// <summary>
    /// Gets and sets the selected season.
    /// </summary>
    public MediaSeason? SelectedSeason
    {
        get => _selectedSeason;
        set => SetProperty(ref _selectedSeason, value);
    }
    
    /// <inheritdoc cref="SelectedEpisode" />
    private MediaEpisode? _selectedEpisode;

    /// <summary>
    /// Gets and sets the selected episode.
    /// </summary>
    public MediaEpisode? SelectedEpisode
    {
        get => _selectedEpisode;
        set => SetProperty(ref _selectedEpisode, value);
    }

    /// <inheritdoc cref="IsTvSeries" />
    private bool _isTvSeries;

    /// <summary>
    /// Gets if the selected media item is a TV series.
    /// </summary>
    public bool IsTvSeries
    {
        get => _isTvSeries;
        private set => SetProperty(ref _isTvSeries, value);
    }
    
    /// <summary>
    /// Executes a search with the <see cref="SearchText"/>.
    /// </summary>
    public async Task SearchAsync()
    {
        SearchResults.Clear();
        SelectedMediaItem = null;
        if (string.IsNullOrEmpty(SearchText)) return;

        IsLoading = true;
        try
        {
            var results = await _mediaLookupService.SearchAsync(SearchText);
            foreach (var result in results)
            {
                SearchResults.Add(result);
            }

            SelectedMediaItem = SearchResults.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching media results.");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Clears all input fields
    /// </summary>
    public void Clear()
    {
        SearchText = "";
        SearchResults.Clear();
        SelectedMediaItem = null;
        SelectedSeason = null;
        SelectedEpisode = null;
    }
    
    /// <summary>
    /// Fetches the media details.
    /// </summary>
    private async Task FetchDetailsAsync()
    {
        Seasons.Clear();
        Episodes.Clear();
        SelectedSeason = null;
        SelectedEpisode = null;
        IsTvSeries = false;
        if (SelectedMediaItem is null) return;
        
        IsLoading = true;
        try
        {
            var details = await _mediaLookupService.GetDetailsAsync(SelectedMediaItem);

            foreach (var season in details.Seasons)
            {
                Seasons.Add(season);
            }

            IsTvSeries = details.MediaType == MediaType.Tv;
            SelectedSeason = Seasons.FirstOrDefault(s => s.SeasonNumber == 1);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching details.");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Fetches the season details.
    /// </summary>
    private async Task FetchSeasonAsync()
    {
        Episodes.Clear();
        SelectedEpisode = null;
        if (SelectedSeason is null) return;
        
        IsLoading = true;
        try
        {
            var details = await _mediaLookupService.GetSeasonDetailsAsync(SelectedSeason);

            foreach (var episode in details.Episodes)
            {
                Episodes.Add(episode);
            }
            
            SelectedEpisode = Episodes.FirstOrDefault(e => e.EpisodeNumber == 1);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching season details.");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Gets the output media info from the selected media item, season and episode.
    /// </summary>
    /// <param name="mediaInfo">Returns the media info if selected.</param>
    /// <returns>Returns true, it a media info was selected.</returns>
    public bool TryGetMediaInfo([MaybeNullWhen(false)] out OutputMediaInfo mediaInfo)
    {
        mediaInfo = null;
        if (SelectedMediaItem is null) return false;

        switch (SelectedMediaItem.MediaType)
        {
            case MediaType.Tv:
                if (SelectedEpisode is null) return false;
                mediaInfo = new OutputMediaInfo
                {
                    Name = SelectedEpisode.Name,
                    Type = OutputMediaType.Episode,
                    Season = SelectedEpisode.SeasonNumber,
                    Episode = SelectedEpisode.EpisodeNumber
                };
                return true;
            case MediaType.Movie:
                mediaInfo = new OutputMediaInfo
                {
                    Name = SelectedMediaItem.Name,
                    Type = OutputMediaType.Movie
                };
                return true;
            default:
                return false;
        }
    }
    
    /// <summary>
    /// Moves to the next episode.
    /// </summary>
    public void IncreaseEpisodeNumber()
    {
        if (SelectedEpisode is null) return;

        var episodeNumber = SelectedEpisode.EpisodeNumber;
        var nextEpisode = Episodes.FirstOrDefault(e => e.EpisodeNumber == episodeNumber + 1);
        SelectedEpisode = nextEpisode;
    }
    
    /// <inheritdoc />
    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        switch (e.PropertyName)
        {
            case nameof(SelectedMediaItem):
                _ = FetchDetailsAsync();
                break;
            case nameof(SelectedSeason):
                _ = FetchSeasonAsync();
                break;
        }
    }

    /// <inheritdoc />
    public override Control CreateView()
    {
        return new MediaLookupView();
    }
}