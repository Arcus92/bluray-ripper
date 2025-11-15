using System.Text;
using System.Web;

namespace MediaLib.TheMovieDatabase.Utils;

/// <summary>
/// A builder query builder.
/// </summary>
public class UriQueryBuilder
{
    /// <summary>
    /// The background string builder.
    /// </summary>
    private readonly StringBuilder _builder;
    
    public UriQueryBuilder()
    {
        _builder = new StringBuilder();
    }

    public UriQueryBuilder(string uri)
    {
        _builder = new StringBuilder(uri);
    }
    
    /// <summary>
    /// Set to true if there are no parameters added yet.
    /// </summary>
    private bool _empty = true;

    /// <summary>
    /// Resets the uri builder.
    /// </summary>
    public void Reset()
    {
        _builder.Clear();
        _empty = true;
    }
    
    /// <summary>
    /// Resets the uri builder.
    /// </summary>
    /// <param name="uri">The base uri.</param>
    public void Reset(string uri)
    {
        Reset();
        _builder.Append(uri);
    }
    
    /// <summary>
    /// Adds the given key-value-pair to the query builder.
    /// </summary>
    /// <param name="key">The key of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    public void Add(string key, string value)
    {
        if (_empty)
        {
            _builder.Append('?');
            _empty = false;
        }
        else
        {
            _builder.Append('&');
        }
        
        _builder.Append(HttpUtility.UrlEncode(key));
        _builder.Append('=');
        _builder.Append(HttpUtility.UrlEncode(value));
    }

    /// <summary>
    /// Adds the given key-value-pair to the query builder.
    /// </summary>
    /// <param name="key">The key of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    public void Add(string key, int value)
    {
        Add(key, value.ToString());
    }
    
    /// <summary>
    /// Adds the given key-value-pair to the query builder.
    /// </summary>
    /// <param name="key">The key of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    public void Add(string key, bool value)
    {
        Add(key, value.ToString());
    }
    
    /// <inheritdoc />
    public override string ToString()
    {
        return _builder.ToString();
    }
}