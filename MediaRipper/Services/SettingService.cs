using System;
using System.IO;
using System.Text.Json;
using MediaRipper.Models.Settings;
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
    
    private SettingsData _data = new();
    
    public SettingService(ILogger<SettingService> logger)
    {
        _logger = logger;

        Load();
    }

    /// <inheritdoc />
    public string SourcePath
    {
        get => _data.SourcePath;
        set
        {
            if (_data.SourcePath == value) return;
            _data.SourcePath = value;
            OnSettingsDataChanged();
        }
    }
    
    /// <inheritdoc />
    public string OutputPath
    {
        get => _data.OutputPath;
        set
        {
            if (_data.OutputPath == value) return;
            _data.OutputPath = value;
            OnSettingsDataChanged();
        }
    }

    /// <inheritdoc />
    public void Load()
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
            var data = JsonSerializer.Deserialize<SettingsData>(file);
            if (data is null)
            {
                return;
            }

            _data = data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load settings file!");
        }
    }

    /// <inheritdoc />
    public void Save()
    {
        try
        {
            _logger.LogInformation("Writing settings file...");
            
            using var file = File.OpenWrite(_filename);
            JsonSerializer.Serialize(file, _data);
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