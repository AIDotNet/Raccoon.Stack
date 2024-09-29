namespace Raccoon.Stack.Event.RabbitMQ;

public sealed class RabbitMqEventEto
{
    public string FullName { get; set; }

    public ReadOnlyMemory<byte> Data { get; set; }

    public RabbitMqEventEto(string fullName, ReadOnlyMemory<byte> data)
    {
        FullName = fullName;
        Data = data;
    }
}