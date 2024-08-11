using Raccoon.Stack.Data;

namespace Raccoon.Stack.EntityFrameworkCore;

public interface ITypeConvertProvider
{
    T? ConvertTo<T>(string value, IDeserializer? deserializer = null);

    object? ConvertTo(string value, Type type, IDeserializer? deserializer = null);
}