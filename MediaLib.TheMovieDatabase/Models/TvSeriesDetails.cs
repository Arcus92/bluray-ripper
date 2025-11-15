namespace MediaLib.TheMovieDatabase.Models;

/// <summary>
/// The full TV series details.
/// </summary>
[Serializable]
public class TvSeriesDetails
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public string OriginalLanguage { get; init; } = "";
    public string OriginalName { get; init; } = "";
    public string Overview { get; init; } = "";
    public int NumberOfEpisodes { get; init; }
    public int NumberOfSeasons { get; init; }
    public DateTime FirstAirDate { get; init; }
    public TvSeason[] Seasons { get; init; } = [];
}