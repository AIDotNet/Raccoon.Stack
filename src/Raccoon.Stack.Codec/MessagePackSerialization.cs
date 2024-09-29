using MessagePack;

namespace Raccoon.Stack.Codec;

public sealed class MessagePackSerialization : ISerialization
{
    private readonly MessagePackSerializerOptions _options;

    public MessagePackSerialization(MessagePackSerializerOptions options)
    {
        _options = options;
    }

    public byte[] Serialize<T>(T message)
    {
        return MessagePackSerializer.Serialize(message, _options);
    }

    public ReadOnlyMemory<byte> SerializeToMemory<T>(T message)
    {
        return MessagePackSerializer.Serialize(message);
    }

    public T Deserialize<T>(byte[] data)
    {
        return MessagePackSerializer.Deserialize<T>(data, _options);
    }

    public T Deserialize<T>(ReadOnlyMemory<byte> data)
    {
        return MessagePackSerializer.Deserialize<T>(data);
    }

    public object? Deserialize(byte[] data, Type type)
    {
        return MessagePackSerializer.Deserialize(type, data, _options) ?? default;
    }

    public object? Deserialize(ReadOnlyMemory<byte> data, Type type)
    {
        return MessagePackSerializer.Deserialize(type, data) ?? default;
    }
}