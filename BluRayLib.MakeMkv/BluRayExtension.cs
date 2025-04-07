using BluRayLib;

namespace MakeMkvLib;

public static class BluRayExtension
{
    /// <summary>
    /// Returns a decryption stream of the given M2TS file with the given clip id: *****.m2ts
    /// </summary>
    /// <param name="bluRay">The BluRay instance.</param>
    /// <param name="clipId">The clip id.</param>
    /// <returns>Returns the decryption stream.</returns>
    public static MakeMkvDecryptStream GetDecryptM2TsStream(this BluRay bluRay, uint clipId)
    {
        return MakeMkvDecryptStream.Open(bluRay.DiskPath, FileName.M2TS(clipId));
    }
}