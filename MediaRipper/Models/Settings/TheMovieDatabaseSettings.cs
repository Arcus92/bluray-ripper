using System;

namespace MediaRipper.Models.Settings;

[Serializable]
public class TheMovieDatabaseSettings
{
    /// <summary>
    /// Gets and sets the api key.
    /// </summary>
    public string ApiKey { get; set; } = "";
    
    /// <summary>
    /// Gets and sets the default language.
    /// </summary>
    public string Language { get; set; } = "";
}