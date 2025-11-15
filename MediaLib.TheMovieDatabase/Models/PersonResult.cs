namespace MediaLib.TheMovieDatabase.Models;

/// <summary>
/// The search result for a person.
/// </summary>
[Serializable]
public class PersonResult : SearchResult
{
    /// <summary>
    /// The name of the person.
    /// </summary>
    public string Name { get; init; } = "";
    
    /// <summary>
    /// The name of the person in the original language.
    /// </summary>
    public string OriginalName { get; init; } = "";
}