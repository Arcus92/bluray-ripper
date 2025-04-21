using System;
using System.Collections.Generic;
using System.Linq;

namespace BluRayRipper.Models;

/// <summary>
/// A list of <see cref="EnumModel{T}"/>s.
/// </summary>
/// <typeparam name="T"></typeparam>
public class EnumModelList<T> : List<EnumModel<T>> where T : struct, IConvertible
{
    /// <summary>
    /// Gets the model for the given value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public EnumModel<T> GetModel(T value)
    {
        return this.FirstOrDefault(s => s.Value.Equals(value));
    }
}