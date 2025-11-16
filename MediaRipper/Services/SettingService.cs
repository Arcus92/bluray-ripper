using System;
using System.IO;
using System.Text.Json;
using MediaRipper.Models.Settings;
using MediaRipper.Serializer;
using MediaRipper.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace MediaRipper.Services;

/// <summary>
/// The service to load and save application settings.
/// </summary>
public class SettingService : ISettingService
{
    private readonly ILogger _logger;
    private readonly string _filename = "settings.json";

    /// <inheritdoc />
    public SettingsData Data { get; private set; } = new();

    public SettingService(ILogger<SettingService> logger)
    {
        _logger = logger;

        Load();
    }

    /// <inheritdoc />
    public void NotifyChange()
    {
        Save();
    }

    /// <summary>
    /// Loads the settings file.
    /// </summary>
    private void Load()
    {
        try
        {
            _logger.LogInformation("Loading settings file...");
            
            if (!File.Exists(_filename))
            {
                _logger.LogInformation("No settings file found. Creating a new file...");
                Save();
                return;
            }
            
            using var file = File.OpenRead(_filename);
            var data = JsonSerializer.Deserialize(file, SettingsContext.Default.SettingsData);
            if (data is null)
            {
                return;
            }

            Data = data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load settings file!");
        }
    }

    /// <summary>
    /// Saves the settings file.
    /// </summary>
    private void Save()
    {
        try
        {
            _logger.LogInformation("Writing settings file...");
            
            using var file = File.Create(_filename);
            JsonSerializer.Serialize(file, Data, SettingsContext.Default.SettingsData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save settings file!");
        }
    }
    
    private void OnSettingsDataChanged()
    {
        Save();
    }
}