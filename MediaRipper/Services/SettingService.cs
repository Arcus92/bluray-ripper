using System;
using MediaRipper.Services.Interfaces;

namespace MediaRipper.Services;

/// <summary>
/// The service to load and save application settings.
/// </summary>
public class SettingService : ISettingService
{
    private readonly string _lastDiskPath;
    private readonly string _lastOutputPath;

    public SettingService()
    {
        _lastDiskPath = "";
        _lastOutputPath = "";

        // For testing, load from command line.
        var args = Environment.GetCommandLineArgs();
        if (args.Length > 1)
        {
            _lastDiskPath = args[1];
        }
        if (args.Length > 2)
        {
            _lastOutputPath = args[2];
        }
    }

    /// <inheritdoc />
    public string GetDefaultDiskPath() => _lastDiskPath;

    /// <inheritdoc />
    public string GetDefaultOutputPath() => _lastOutputPath;
}