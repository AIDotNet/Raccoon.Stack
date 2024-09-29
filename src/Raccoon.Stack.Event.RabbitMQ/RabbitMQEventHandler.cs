using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client.Events;
using Raccoon.Stack.Codec;
using Raccoon.Stack.Rabbit;
using Raccoon.Stack.Rabbit.Handler;

namespace Raccoon.Stack.Event.RabbitMQ;

public sealed class RabbitMQEventHandler : IRabbitHandler
{
    private readonly ILogger<RabbitMQEventHandler> _logger;
    private readonly ISerialization _serialization;
    private readonly string _queue;

    private static readonly ConcurrentDictionary<string, Type> TypeCache = new();

    public RabbitMQEventHandler(
        IOptions<RabbitMQOptions> options,
        ILogger<RabbitMQEventHandler> logger,
        ISerialization serialization)
    {
        _logger = logger;
        _serialization = serialization;
        _queue = options.Value.Prefix + "EventBus";
    }

    public bool Enable(ConsumeOptions options)
    {
        return options.Queue.Equals(_queue, StringComparison.OrdinalIgnoreCase);
    }

    public async Task Handle(IServiceProvider sp, BasicDeliverEventArgs args, ConsumeOptions options)
    {
        try
        {
            var eto = _serialization.Deserialize<RabbitMqEventEto>(args.Body);

            var type = TypeCache.GetOrAdd(eto.FullName,
                fullName => Assembly.GetEntryAssembly()?.GetType(fullName) ?? null);

            if (type == null)
            {
                _logger.LogWarning("Event type not found: {FullName}", eto.FullName);
                return;
            }

            var value = _serialization.Deserialize(eto.Data, type);

            var handlerType = typeof(IEventHandler<>).MakeGenericType(type);

            var handler = sp.GetService(handlerType);

            if (handler == null)
            {
                _logger.LogWarning("Handler not found for event type: {FullName}", eto.FullName);
                return;
            }

            var method = handlerType.GetMethod("HandleAsync");

            await (Task)method?.Invoke(handler, new[] { value });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error processing event: {Message}", e.Message);
        }
    }
}