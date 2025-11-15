using MediaLib.TheMovieDatabase;

namespace MediaRipper.Services.Interfaces;

public interface IMediaLookupService
{
    /// <summary>
    /// Gets TheMovieDatabase api service.
    /// </summary>
    TheMovieDatabaseApi MovieDatabaseApi { get; }
    
    /// <summary>
    /// Gets the default language.
    /// </summary>
    string Language { get; }
}