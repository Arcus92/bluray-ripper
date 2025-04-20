using System.Security.Cryptography;

namespace BluRayLib.Utils;

/// <summary>
/// Class to calculate a TheDiscDb compatible content hash from the given source.
/// </summary>
public static class ContentHash
{
    /// <summary>
    /// The relevant file information to calculate the content hash.
    /// </summary>
    public readonly struct HashFileInfo
    {
        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        public string Name { get; init; }
        
        /// <summary>
        /// Gets the creation date of the file.
        /// </summary>
        public DateTime CreationTime { get; init; }
        
        /// <summary>
        /// Gets the size of the file.
        /// </summary>
        public long Size { get; init; }
    }
    
    /// <summary>
    /// Calculates a TheDiscDb compatible content hash of the given input tiles. 
    /// </summary>
    /// <param name="files">The files to hash.</param>
    /// <returns>Returns the content hash.</returns>
    public static string CalculateHash(this IEnumerable<HashFileInfo> files)
    {
        using var md5 = MD5.Create();
        foreach (var file in files)
        {
            var bytes = BitConverter.GetBytes(file.Size);
            md5.TransformBlock(bytes, 0, bytes.Length, new byte[bytes.Length], 0);
        }
        
        md5.TransformFinalBlock([], 0, 0);

        if (md5.Hash is null)
        {
            throw new Exception("Failed to calculate content hash!");
        }
        
        return Convert.ToHexString(md5.Hash);
    }
}