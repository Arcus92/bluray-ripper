namespace MediaLib.TheMovieDatabase.Models;

/// <summary>
/// The search result for a company.
/// </summary>
[Serializable]
public class CompanyResult : SearchResult
{
    /// <summary>
    /// Gets the company name.
    /// </summary>
    public string Name { get; init; } = "";
    
    /// <summary>
    /// Gets the url path to the logo image.
    /// </summary>
    public string? LogoPath { get; init; }
    
    /// <summary>
    /// Gets the origin country id.
    /// </summary>
    public string OriginCountry { get; init; } = "";
}