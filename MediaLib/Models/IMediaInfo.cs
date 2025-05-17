namespace MediaLib.Models;

public interface IMediaInfo
{
    /// <summary>
    /// Gets the media id.
    /// </summary>
    ushort Id { get; }
    
    /// <summary>
    /// Gets the media name.
    /// </summary>
    string Name { get; }
}