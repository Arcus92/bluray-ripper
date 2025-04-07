namespace MakeMkvLib;

/// <summary>
/// The MakeMkv filename flags.
/// </summary>
[Flags]
public enum FileNameFlags : uint
{
    /// <summary>
    /// M2TS file, default value
    /// </summary>
    M2TS = 0,
    
    /// <summary>
    /// SSIF file
    /// </summary>
    SSIF = 0x80000000,
    
    /// <summary>
    /// only apply AACS decryption, skip BD+
    /// </summary>
    AACSOnly = 0x00100000,
    
    /// <summary>
    /// only apply BD+ transform
    /// </summary>
    BDPlusOnly = 0x00200000,
    
    /// <summary>
    /// only apply BUS decryption, skip BD+ and AACS
    /// </summary>
    BusOnly = 0x00400000,
    
    /// <summary>
    /// do not decrypt, get an AACS block key instead, must use with MMBD_FLAG_AACS_ONLY
    /// </summary>
    BlockKey = 0x00800000,
    
    /// <summary>
    /// for libaacs compatibility
    /// </summary>
    AutoCPSID = 0x10000000
}

/// <summary>
/// Helper class to create MakeMkv filename flags.
/// </summary>
public static class FileName
{
    /// <summary>
    /// Returns a M2TS file name: *****.m2ts
    /// </summary>
    /// <param name="clipId">The file id.</param>
    /// <returns></returns>
    public static FileNameFlags M2TS(uint clipId)
    {
        return FileNameFlags.M2TS | (FileNameFlags)clipId;
    }
    
    /// <summary>
    /// Returns a SSIF file name: *****.ssif
    /// </summary>
    /// <param name="clipId">The file id.</param>
    /// <returns></returns>
    public static FileNameFlags SSIF(uint clipId)
    {
        return FileNameFlags.SSIF | (FileNameFlags)clipId;
    }

    /// <summary>
    /// Gets the clip if from the given name flags.
    /// </summary>
    /// <param name="nameFlags">The name flags to a specific file.</param>
    /// <returns></returns>
    public static uint GetClipId(this FileNameFlags nameFlags)
    {
        // TODO: Check for SSIF file.
        return (uint)nameFlags;
    }
    
    /// <summary>
    /// Returns the relative file 
    /// </summary>
    /// <param name="nameFlags">The name flags to a specific file.</param>
    /// <returns></returns>
    public static string GetLocalPath(this FileNameFlags nameFlags)
    {
        var clipId = nameFlags.GetClipId();
        return $"BDMV/STREAM/{clipId:00000}.m2ts";
    }
}