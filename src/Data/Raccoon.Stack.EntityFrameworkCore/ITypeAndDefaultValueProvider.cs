using System.Diagnostics.CodeAnalysis;

namespace Raccoon.Stack.EntityFrameworkCore;

public interface ITypeAndDefaultValueProvider
{
    /// <summary>
    /// Setting type and default value
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    bool TryAdd(Type type);

    bool TryGet(Type type, [NotNullWhen(true)] out string? defaultValue);
}
