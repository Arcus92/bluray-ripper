using System.Runtime.InteropServices;

namespace MakeMkvLib;

/// <summary>
/// A wrapper class for the libmmbd from MakeMkv. This library handles BluRay file encryption by launching MakeMkv in
/// the background. <para/>
/// MakeMkv must be installed and registered on the machine. 
/// </summary>
public partial class MakeMkv : IDisposable
{
    /// <summary>
    /// The internal pointer to the MMDB context.
    /// </summary>
    private IntPtr _ptr;

    /// <summary>
    /// Creates a new MakeMkv context.
    /// </summary>
    /// <param name="userContext">Custom data pointer that can be used in the <paramref name="outputProc"/> callback.</param>
    /// <param name="outputProc">Custom message handler.</param>
    /// <param name="args">Additional arguments for MakeMkv.</param>
    public MakeMkv(IntPtr userContext = 0, OutputProc? outputProc = null, string? args = null)
    {
        _ptr = NativeCreateContext(userContext, outputProc, args);
        if (_ptr == IntPtr.Zero)
            throw new Exception("Failed to create MakeMkv context!");
    }

    /// <summary>
    /// The buffer size 
    /// </summary>
    public const int UnitSize = 6144;
    
    private bool _isOpen = false;
    
    /// <summary>
    /// Opens a BluRay disk by path.
    /// </summary>
    /// <param name="path">The path to the BluRay disk root directory.</param>
    /// <returns>Returns the MakeMkv error code. If successful, 0 is returned.</returns>
    public int Open(string path)
    {
        if (_isOpen) Close();
        
        var status = NativeOpen(_ptr, path);
        if (status == 0) _isOpen = true;
        return status;
    }
    
    /// <summary>
    /// Closes the current open BluRay disk opened via <see cref="Open"/>.
    /// </summary>
    /// <returns>Returns the MakeMkv error code. If successful, 0 is returned.</returns>
    public int Close()
    {
        _isOpen = false;
        return NativeClose(_ptr);
    }

    /// <summary>
    /// Gets the MKB version of the open disk.
    /// </summary>
    /// <returns>Returns the MKB version.</returns>
    public int GetMkbVersion()
    {
        return NativeGetMkbVersion(_ptr);
    }

    /// <summary>
    /// Decrypts the given buffer of the given file. The buffer is overwritten in the process.
    /// </summary>
    /// <param name="nameFlags">The filename flag linking to the file to decrypt.</param>
    /// <param name="fileOffset">The current in file position of the given buffer.</param>
    /// <param name="buffer">The buffer holding the encrypted data. This will be overwritten with the decrypted data. The buffer must be exactly 6144 bytes.</param>
    /// <returns>Returns the MakeMkv error code. If successful, 0 is returned.</returns>
    /// <exception cref="ArgumentException">Is thrown, if the buffer isn't 6144 bytes in size.</exception>
    public int DecryptUnit(FileNameFlags nameFlags, ulong fileOffset, byte[] buffer)
    {
        if (buffer.Length != UnitSize)
            throw new ArgumentException($"Buffer size must be {UnitSize}!");
        return NativeDecryptUnit(_ptr, nameFlags, fileOffset, buffer);
    }
    
    /// <summary>
    /// Gets the version text of MakeMkv.
    /// </summary>
    /// <returns></returns>
    public static string? GetVersionString()
    {
        var ptr = NativeGetVersionString();
        return Marshal.PtrToStringAnsi(ptr);
    }
    
    #region IDisposable
    
    /// <inheritdoc />
    public void Dispose()
    {
        if (_ptr == IntPtr.Zero) return;
        if (_isOpen) Close();
        NativeDestroyContext(_ptr);
        _ptr = IntPtr.Zero;
    }
    
    #endregion IDisposable
    
    #region Native
    
    private const string LibraryName = "libmmbd.so.0";

    /// <summary>
    /// The native output message handler for MakeMkv. 
    /// </summary>
    public delegate void OutputProc(IntPtr userContext, uint flags, [MarshalAs(UnmanagedType.LPStr)] string message, IntPtr nullPtr);
    
    [LibraryImport(LibraryName, EntryPoint = "mmbd_get_version_string")]
    private static partial IntPtr NativeGetVersionString();
    
    [LibraryImport(LibraryName, EntryPoint = "mmbd_create_context")]
    private static partial IntPtr NativeCreateContext(IntPtr userContext = 0, OutputProc? outputProc = null, [MarshalAs(UnmanagedType.LPStr)] string? args = null);
    
    [LibraryImport(LibraryName, EntryPoint = "mmbd_open")]
    private static partial int NativeOpen(IntPtr context, [MarshalAs(UnmanagedType.LPStr)] string path);
    
    [LibraryImport(LibraryName, EntryPoint = "mmbd_close")]
    private static partial int NativeClose(IntPtr context);
    
    [LibraryImport(LibraryName, EntryPoint = "mmbd_get_mkb_version")]
    private static partial int NativeGetMkbVersion(IntPtr context);
    
    [LibraryImport(LibraryName, EntryPoint = "mmbd_decrypt_unit")]
    private static partial int NativeDecryptUnit(IntPtr context, FileNameFlags nameFlags, ulong fileOffset, [In][Out] byte[] buffer);
    
    [LibraryImport(LibraryName, EntryPoint = "mmbd_destroy_context")]
    private static partial void NativeDestroyContext(IntPtr context);
    
    #endregion Native
}