using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using MediaLib.TheMovieDatabase.Converters;
using MediaLib.TheMovieDatabase.Services;

namespace MediaLib.TheMovieDatabase;

/// <summary>
/// The api for TheMovieDatabase.
/// You'll need to set the <see cref="_apiKey"/> before using the services.
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
    private readonly string _apiKey;

    public TheMovieDatabaseApi(IHttpClientFactory httpClientFactory, string apiKey)
    {
        _httpClientFactory = httpClientFactory;
        _apiKey = apiKey;

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
    /// The JSON serializer options.
    /// </summary>
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        AllowOutOfOrderMetadataProperties = true,
        Converters =
        {
            new JsonStringEnumConverter(),
            new DateTimeFormatConverter()
        }
    };
    
    /// <summary>
    /// Sends a GET request to the TMDB api.
    /// </summary>
    /// <param name="uri">The URI to fetch.</param>
    /// <typeparam name="T">The response type.</typeparam>
    /// <returns>Returns the serialized response from JSON.</returns>
    internal async Task<T> GetAsync<T>(string uri)
    {
        var client = _httpClientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        AddHeaders(request);
        
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<T>(JsonSerializerOptions);
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
    private void AddHeaders(HttpRequestMessage request)
    {
        request.Headers.Add("Accept", "application/json");
        request.Headers.Add("Authorization", $"Bearer {_apiKey}");
    }
}