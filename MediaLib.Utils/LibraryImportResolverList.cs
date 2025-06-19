using System.Reflection;
using System.Runtime.InteropServices;

namespace MediaLib.Utils;

/// <summary>
/// A handler for multiple library import resolvers. The resolvers are handled in order of attachment.
/// </summary>
public class LibraryImportResolverList
{
    /// <summary>
    /// The list of stored resolvers.
    /// </summary>
    private readonly List<DllImportResolver> _resolvers = new();
    
    /// <summary>
    /// Adds a new resolver.
    /// </summary>
    /// <param name="resolver">The resolver to add.</param>
    public void Add(DllImportResolver resolver)
    {
        _resolvers.Add(resolver);
    }
    
    /// <summary>
    /// Registers this import resolver.
    /// </summary>
    /// <param name="assembly">The assembly to register to.</param>
    public void Register(Assembly assembly)
    {
        NativeLibrary.SetDllImportResolver(assembly, Resolve);
    }
    
    private IntPtr Resolve(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        foreach (var resolver in _resolvers)
        {
            var ret = resolver(libraryName, assembly, searchPath);
            if (ret != IntPtr.Zero)
                return ret;
        }
        
        return IntPtr.Zero;
    }
    
    #region Static

    /// <summary>
    /// The shared instance.
    /// </summary>
    private static readonly Dictionary<Assembly, LibraryImportResolverList> ResolverPerAssembly = new();

    /// <summary>
    /// Gets a shared global resolver instance.
    /// This will create a new instance and register it via <see cref="NativeLibrary.SetDllImportResolver"/> if needed.
    /// </summary>
    /// <param name="assembly">The assembly to register to.</param>
    public static LibraryImportResolverList GetGlobalResolver(Assembly assembly)
    {
        if (ResolverPerAssembly.TryGetValue(assembly, out var resolver))
        {
            return resolver;
        }
        
        resolver = new LibraryImportResolverList();
        resolver.Register(assembly);
        ResolverPerAssembly.Add(assembly, resolver);
        return resolver;
    }

    /// <summary>
    /// Registers a new library resolver globally.
    /// </summary>
    /// <param name="assembly">The assembly to register to.</param>
    /// <param name="resolver">The resolver to attach.</param>
    public static void AddGlobalResolver(Assembly assembly, DllImportResolver resolver)
    {
        GetGlobalResolver(assembly).Add(resolver);
    }

    #endregion Static
}