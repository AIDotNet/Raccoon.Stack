using System.Text.Json;
using Microsoft.Extensions.Options;
using Raccoon.Stack.Codec;
using Raccoon.Stack.Rabbit;

namespace Raccoon.Stack.Event.RabbitMQ;

public class RabbitMQEventBus<TEvent>(
    RabbitClient rabbitClient,
    IOptions<RabbitMQOptions> options,
    ISerialization serialization)
    : IEventBus<TEvent> where TEvent : class
{
    public async Task PublishAsync(TEvent @event)
    {
        var eto = new RabbitMqEventEto(@event.GetType().FullName, serialization.Serialize(@event));

        await rabbitClient.PublishAsync(options.Value.Prefix + "EventBus:exchange",
            options.Value.Prefix + "key",
            JsonSerializer.SerializeToUtf8Bytes(eto));
    }
}