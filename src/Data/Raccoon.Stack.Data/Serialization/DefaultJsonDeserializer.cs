using System.Text.Json;

namespace Raccoon.Stack.Data;

internal class DefaultJsonDeserializer(JsonSerializerOptions? options = null) : IDeserializer
{
    public TValue? Deserialize<TValue>(string value)
        => JsonSerializer.Deserialize<TValue>(value, options);

    public object? Deserialize(string value, Type valueType)
        => JsonSerializer.Deserialize(value, valueType, options);
}
