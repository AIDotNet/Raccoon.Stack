namespace Raccoon.Stack.Data;

public interface IDeserializer
{
    TValue? Deserialize<TValue>(string value);

    object? Deserialize(string value, Type valueType);
}
