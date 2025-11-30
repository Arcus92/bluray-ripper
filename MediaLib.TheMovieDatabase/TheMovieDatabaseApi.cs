using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using MediaLib.TheMovieDatabase.Services;

namespace MediaLib.TheMovieDatabase;

/// <summary>
/// The api for TheMovieDatabase.
/// You'll need to set the <see cref="ApiKey"/> before using the services.
/// </summary>
public class TheMovieDatabaseApi
{
    /// <summary>
    /// The main api endpoint.
    /// </summary>
    internal string Endpoint => "https://api.themoviedb.org/";
    
    /// <summary>
    /// The http client factory.
    /// </summary>
    private readonly IHttpClientFactory _httpClientFactory;
    
    /// <summary>
    /// Gets and sets teh authentication token.
    /// </summary>
    public string? ApiKey { get; set; }

    public TheMovieDatabaseApi(IHttpClientFactory httpClientFactory, string? apiKey = null)
    {
        _httpClientFactory = httpClientFactory;
        ApiKey = apiKey;

        Search = new SearchService(this);
        Tv = new TvService(this);
        Movie = new MovieService(this);
    }

    /// <summary>
    /// Gets the search service.
    /// </summary>
    public SearchService Search { get; }
    
    /// <summary>
    /// Gets the TV service.
    /// </summary>
    public TvService Tv { get; }
    
    /// <summary>
    /// Gets the movie service.
    /// </summary>
    public MovieService Movie { get; }
    
    /// <summary>
    /// Sends a GET request to the TMDB api.
    /// </summary>
    /// <param name="uri">The URI to fetch.</param>
    /// <param name="typeInfo">The JSON type info.</param>
    /// <typeparam name="T">The response type.</typeparam>
    /// <returns>Returns the serialized response from JSON.</returns>
    internal async Task<T> GetAsync<T>(string uri, JsonTypeInfo<T> typeInfo)
    {
        var client = _httpClientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        ValidateAndAddHeaders(request);
        
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync(content, typeInfo);
        if (result is null)
        {
            throw new HttpRequestException("HTTP request returned null.");
        }
        
        return result;
    }

    /// <summary>
    /// Adds the general HTTP headers.
    /// </summary>
    /// <param name="request">The request to add the HTTP headers.</param>
    private void ValidateAndAddHeaders(HttpRequestMessage request)
    {
        if (string.IsNullOrEmpty(ApiKey))
        {
            throw new ArgumentException("TheMovieDatabase ApiKey is not set.", nameof(ApiKey));
        }
        
        request.Headers.Add("Accept", "application/json");
        request.Headers.Add("Authorization", $"Bearer {ApiKey}");
    }
}