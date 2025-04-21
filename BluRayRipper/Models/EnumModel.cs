using System;
using Avalonia;

namespace BluRayRipper.Models;

/// <summary>
/// A model to store a display text and an enum value for a view.
/// </summary>
/// <typeparam name="T">The enum value.</typeparam>
public readonly struct EnumModel<T> : IEquatable<EnumModel<T>> where T : struct, IConvertible
{
    public EnumModel(T value, string resourceName)
    {
        Value = value;
        ResourceName = resourceName;
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    public T Value { get; }
    
    /// <summary>
    /// Gets the resource name.
    /// </summary>
    public string ResourceName { get; }

    /// <summary>
    /// Gets the display name.
    /// </summary>
    public string DisplayName
    {
        get
        {
            if (Application.Current is not null &&
                Application.Current.Resources.TryGetValue(ResourceName, out var resource) && resource is not null)
            {
                return resource.ToString() ?? "";
            }

            return ResourceName;
        }
    }
    
    /// <inheritdoc />
    public override string ToString() => DisplayName;

    /// <inheritdoc />
    public bool Equals(EnumModel<T> other)
    {
        return Value.Equals(other.Value);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is EnumModel<T> other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}