using System.Text.Json.Serialization;

namespace MediaLib.TheMovieDatabase.Models;

/// <summary>
/// Base class for search results.
/// </summary>
[Serializable]
[JsonPolymorphic(TypeDiscriminatorPropertyName = "media_type", UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToBaseType, IgnoreUnrecognizedTypeDiscriminators = true)]
[JsonDerivedType(typeof(MovieResult), "movie")]
[JsonDerivedType(typeof(TvResult), "tv")]
[JsonDerivedType(typeof(CompanyResult), "company")]
[JsonDerivedType(typeof(PersonResult), "person")]
public class SearchResult
{
    /// <summary>
    /// Gets the id of the media item.
    /// </summary>
    public int Id { get; init; }
}