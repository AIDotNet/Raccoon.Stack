using RabbitMQ.Client.Events;

namespace BSI.Stack.Rabbit.Handler;

public interface IRabbitEventBus
{
    Task Trigger(IServiceProvider sp, BasicDeliverEventArgs args, ConsumeOptions options);
}