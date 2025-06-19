using System.Reflection;
using System.Runtime.InteropServices;
using MediaLib.Utils;

namespace DvdLib.Decrypt;

public partial class DvdCss
{
    public const int BlockSize = 2048;
    
    #region Native
    
    private const string LibraryName = "libdvdcss";
    
    [LibraryImport(LibraryName, EntryPoint = "dvdcss_open")]
    private static partial IntPtr NativeOpen([MarshalAs(UnmanagedType.LPStr)] string path);
    
    [LibraryImport(LibraryName, EntryPoint = "dvdcss_read")]
    private static partial int NativeRead(IntPtr context, byte[] buffer, int blocks, int flags);
    
    [LibraryImport(LibraryName, EntryPoint = "dvdcss_seek")]
    private static partial int NativeSeek(IntPtr context, int block, int flags);
    
    [LibraryImport(LibraryName, EntryPoint = "dvdcss_close")]
    private static partial int NativeClose(IntPtr context);
    
    [LibraryImport(LibraryName, EntryPoint = "dvdcss_is_scrambled")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool NativeIsScrambled(IntPtr context);
    
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
            libraryName = "libdvdcss.so.0";
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
}