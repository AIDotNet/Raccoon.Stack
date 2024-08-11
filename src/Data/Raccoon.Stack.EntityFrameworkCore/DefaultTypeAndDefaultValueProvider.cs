using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Raccoon.Stack.EntityFrameworkCore;

public class DefaultTypeAndDefaultValueProvider : ITypeAndDefaultValueProvider
{
    private readonly ConcurrentDictionary<Type, string?> _typeAndDefaultValues;

    public DefaultTypeAndDefaultValueProvider()
    {
        _typeAndDefaultValues = new();
        _typeAndDefaultValues.AddOrUpdate(typeof(string), _ => string.Empty, (_, _) => string.Empty);
    }

    public bool TryAdd(Type type)
    {
        return _typeAndDefaultValues.TryAdd(type, Activator.CreateInstance(type)?.ToString());
    }

    public bool TryGet(Type type, [NotNullWhen(true)] out string? defaultValue)
    {
        return _typeAndDefaultValues.TryGetValue(type, out defaultValue);
    }
}