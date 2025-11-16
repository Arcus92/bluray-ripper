using System.Text.Json.Serialization;
using MediaLib.TheMovieDatabase.Models;

namespace MediaLib.TheMovieDatabase.Serializer;

[JsonSourceGenerationOptions(
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower,
    AllowOutOfOrderMetadataProperties = true,
    Converters = [ 
        typeof(DateTimeFormatConverter) 
    ]
)]
[JsonSerializable(typeof(CompanyResult))]
[JsonSerializable(typeof(MovieResult))]
[JsonSerializable(typeof(TvResult))]
[JsonSerializable(typeof(PersonResult))]
[JsonSerializable(typeof(SearchResult))]
[JsonSerializable(typeof(SearchResults<SearchResult>))]
[JsonSerializable(typeof(SearchResults<MovieResult>))]
[JsonSerializable(typeof(SearchResults<TvResult>))]
[JsonSerializable(typeof(SearchResults<MovieDetails>))]
[JsonSerializable(typeof(SearchResults<TvEpisodeDetails>))]
[JsonSerializable(typeof(SearchResults<TvSeasonDetails>))]
[JsonSerializable(typeof(SearchResults<TvSeriesDetails>))]
public partial class ModelContext : JsonSerializerContext
{
}