using Avalonia.Platform.Storage;

namespace MediaRipper.Services.Interfaces;

public interface IStorageProviderAccessor
{
    /// <summary>
    /// Gets the storage provider.
    /// </summary>
    IStorageProvider? StorageProvider { get; set; }
}