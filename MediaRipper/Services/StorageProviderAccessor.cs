using Avalonia.Platform.Storage;
using MediaRipper.Services.Interfaces;

namespace MediaRipper.Services;

/// <summary>
/// The service to access the <see cref="IStorageProvider"/>.
/// </summary>
public class StorageProviderAccessor : IStorageProviderAccessor
{
    /// <inheritdoc />
    public IStorageProvider? StorageProvider { get; set; }
}