using System.Text.Json;

namespace Raccoon.Stack.Data;

internal class DefaultJsonSerializer : ISerializer
{
    private readonly JsonSerializerOptions? _options;

    public DefaultJsonSerializer(JsonSerializerOptions? options = null) => _options = options;

    public string Serialize<TValue>(TValue value) => JsonSerializer.Serialize(value, _options);
}