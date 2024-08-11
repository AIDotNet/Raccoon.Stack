using RabbitMQ.Client.Events;

namespace Raccoon.Stack.Rabbit.Handler;

public interface IRabbitEventBus
{
    Task Trigger(IServiceProvider sp, BasicDeliverEventArgs args, ConsumeOptions options);
}