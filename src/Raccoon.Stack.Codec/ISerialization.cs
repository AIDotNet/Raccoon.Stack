namespace Raccoon.Stack.Codec;

public interface ISerialization
{
    byte[] Serialize<T>(T message);

    ReadOnlyMemory<byte> SerializeToMemory<T>(T message);

    T Deserialize<T>(byte[] data);

    T Deserialize<T>(ReadOnlyMemory<byte> data);
    
    /// <summary>
    /// Deserialize the data to the specified type.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    object? Deserialize(byte[] data, Type type);
    
    /// <summary>
    /// Deserialize the data to the specified type.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    object? Deserialize(ReadOnlyMemory<byte> data, Type type);
}