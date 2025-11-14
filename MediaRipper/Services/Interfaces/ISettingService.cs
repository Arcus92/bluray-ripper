using MediaRipper.Models.Settings;

namespace MediaRipper.Services.Interfaces;

public interface ISettingService
{
    /// <summary>
    /// Gets the current loaded settings data.
    /// </summary>
    SettingsData Data { get; }

    /// <summary>
    /// Notifies the service that the <see cref="Data"/> was changed.
    /// </summary>
    void NotifyChange();
}