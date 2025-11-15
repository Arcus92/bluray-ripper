namespace MediaLib.TheMovieDatabase.Models;

/// <summary>
/// The search result list.
/// </summary>
/// <typeparam name="T">The type of the search result.</typeparam>
[Serializable]
public class SearchResults<T>
{
    /// <summary>
    /// Gets the index of the current page. Starting at 1.
    /// </summary>
    public int Page { get; init; }
    
    /// <summary>
    /// Gets the result items.
    /// </summary>
    public T[] Results { get; init; } = [];
    
    /// <summary>
    /// Gets the number of total pages.
    /// </summary>
    public int TotalPages { get; init; }
    
    /// <summary>
    /// Gets the number of total results.
    /// </summary>
    public int TotalResults { get; init; }
}