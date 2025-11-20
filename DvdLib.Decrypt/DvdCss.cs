using System.Reflection;
using System.Runtime.InteropServices;
using MediaLib.Utils;

namespace DvdLib.Decrypt;

public partial class DvdCss : IDisposable
{
    public const int BlockSize = 2048;

    private IntPtr _ptr;

    /// <summary>
    /// Opens the DVD path.
    /// </summary>
    /// <param name="path">The path to the DVD path or file.</param>
    /// <returns>Returns if the DVD could be opened.</returns>
    public bool Open(string path)
    {
        var ptr = NativeOpen(path);
        if (ptr == -1)
        {
            return false;
        }
        
        _ptr = ptr;
        return true;
    }

    /// <summary>
    /// Closes the DVD handle.
    /// </summary>
    /// <returns>Returns if the operation was successful.</returns>
    public bool Close()
    {
        if (_ptr == IntPtr.Zero) return true;
        var result = NativeClose(_ptr);
        _ptr = IntPtr.Zero;
        return result >= 0;
    }
    
    /// <summary>
    /// Reads the number of blocks and writes them to the buffer.
    /// </summary>
    /// <param name="buffer">The buffer to write to.</param>
    /// <param name="blocks">The number of blocks to read.</param>
    /// <param name="flags">Additional flags.</param>
    /// <returns>Returns the number of read blocks.</returns>
    public int Read(byte[] buffer, int blocks, DvdCssReadFlags flags = DvdCssReadFlags.None)
    {
        if (_ptr == IntPtr.Zero) return -1;
        return NativeRead(_ptr, buffer, blocks, flags);
    }

    /// <summary>
    /// Seeks to the given block position.
    /// </summary>
    /// <param name="block">The block position.</param>
    /// <param name="flags">Additional flags.</param>
    /// <returns>Returns if the operation was successful.</returns>
    public bool Seek(int block, DvdCssSeekFlags flags = DvdCssSeekFlags.None)
    {
        if (_ptr == IntPtr.Zero) return false;
        var result = NativeSeek(_ptr, block, flags);
        return result >= 0;
    }
    
    /// <summary>
    /// Returns if the opened DVD is scrambled.
    /// </summary>
    /// <returns>Returns true if the opened DVD is scrambled.</returns>
    public bool IsScrambled()
    {
        if (_ptr == IntPtr.Zero) return false;
        return NativeIsScrambled(_ptr);
    }

    /// <summary>
    /// Returns the current error message.
    /// </summary>
    /// <returns>The error message.</returns>
    public string Error()
    {
        if (_ptr == IntPtr.Zero) return "";
        return NativeError(_ptr);
    }
    
    /// <summary>
    /// Registers DVDCss as DVD decryption handler.
    /// </summary>
    public static void RegisterAsDecryptionHandler()
    {
        Dvd.VobDecryptionHandler = VobDecryptionHandler;
    }

    private static Stream VobDecryptionHandler(Dvd dvd, string filename) =>
        DvdCssDecryptStream.Open(dvd.DiskPath, filename);
    
    #region Native
    
    private const string LibraryName = "libdvdcss";
    
    [LibraryImport(LibraryName, EntryPoint = "dvdcss_open")]
    private static partial IntPtr NativeOpen([MarshalAs(UnmanagedType.LPStr)] string path);
    
    [LibraryImport(LibraryName, EntryPoint = "dvdcss_read")]
    private static partial int NativeRead(IntPtr context, [Out] byte[] buffer, int blocks, DvdCssReadFlags flags);
    
    [LibraryImport(LibraryName, EntryPoint = "dvdcss_seek")]
    private static partial int NativeSeek(IntPtr context, int block, DvdCssSeekFlags flags);
    
    [LibraryImport(LibraryName, EntryPoint = "dvdcss_close")]
    private static partial int NativeClose(IntPtr context);
    
    [LibraryImport(LibraryName, EntryPoint = "dvdcss_is_scrambled")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool NativeIsScrambled(IntPtr context);

    [LibraryImport(LibraryName, EntryPoint = "dvdcss_error")]
    [return: MarshalAs(UnmanagedType.LPStr)]
    private static partial string NativeError(IntPtr context);
    
    /// <summary>
    /// The library import resolver to handle the name and location of libdvdcss.
    /// </summary>
    /// <param name="libraryName">The loaded library name.</param>
    /// <param name="assembly">The loading assembly.</param>
    /// <param name="searchPath">The search path.</param>
    /// <returns>Returns the loaded library pointer.</returns>
    public static IntPtr LibraryImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (libraryName != LibraryName) 
            return IntPtr.Zero; // Fallback to default resolver

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            libraryName = "libdvdcss.dll";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || 
                 RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
        {
            libraryName = "libdvdcss.so";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            libraryName = "libdvdcss.dylib";
        }
        else return IntPtr.Zero;
        
        return NativeLibrary.Load(libraryName, assembly, searchPath);
    }
    
    /// <summary>
    /// Registers the library import resolve to handle the name and location of libdvdcss.
    /// </summary>
    public static void RegisterLibraryImportResolver()
    {
        LibraryImportResolverList.AddGlobalResolver(Assembly.GetExecutingAssembly(), LibraryImportResolver);
    }
    
    #endregion Native

    #region IDisposable
    
    /// <inheritdoc />
    public void Dispose()
    {
        Close();
    }
    
    #endregion IDisposable
}