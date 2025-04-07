using System.Diagnostics.CodeAnalysis;

namespace BluRayLib.Utils;

public static class LinqHelper
{
    /// <summary>
    /// Returns the first item in the enumerable with the given condition.
    /// Returns false if none item was found.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="predicate">The predicate condition.</param>
    /// <param name="result">Returns the found item.</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool TryFirst<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate,[MaybeNullWhen(false)] out T result)
    {
        foreach (var item in enumerable)
        {
            if (!predicate(item)) continue;
            result = item;
            return true;
        }

        result = default;
        return false;
    }
}