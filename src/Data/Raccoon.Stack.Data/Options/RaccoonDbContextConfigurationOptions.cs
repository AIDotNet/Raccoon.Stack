using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Raccoon.Stack.Data.Options;

public class RaccoonDbContextConfigurationOptions
{
    private readonly ConcurrentDictionary<string, string> _data = new();

    public bool TryGetConnectionString(string name, [NotNullWhen(true)] out string? connectionString)
        => _data.TryGetValue(name, out connectionString);

    public void AddConnectionString(string name, string connectionString)
        => _data.TryAdd(name, connectionString);
}