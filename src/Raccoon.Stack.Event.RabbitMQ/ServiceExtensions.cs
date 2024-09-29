using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Raccoon.Stack.Rabbit;

namespace Raccoon.Stack.Event.RabbitMQ;

public static class ServiceExtensions
{
    public static IServiceCollection AddRabbitMqEventBus(this IServiceCollection services,
        Action<RabbitMQOptions> config)
    {
        var options = new RabbitMQOptions();

        config?.Invoke(options);

        services.Configure<RabbitMQOptions>((mqOptions =>
        {
            mqOptions.ConnectionString = options.ConnectionString;
            mqOptions.FetchCount = options.FetchCount;
            mqOptions.Prefix = options.Prefix;
        }));

        services
            .AddSingleton(typeof(IEventBus<>), typeof(RabbitMQEventBus<>));

        services.AddRabbitBoot((rabbitOptions =>
        {
            rabbitOptions.ConnectionString = options.ConnectionString;
            rabbitOptions.Consumes =
            [
                new ConsumeOptions
                {
                    AutoAck = false,
                    FetchCount = options.FetchCount,
                    Queue = options.Prefix + "EventBus",
                    Declaration = (declaration =>
                    {
                        declaration.QueueDeclareAsync(options.Prefix + "EventBus", true);
                        declaration.ExchangeDeclareAsync(options.Prefix + "EventBus:exchange", ExchangeType.Direct,
                            true);
                        declaration.QueueBindAsync(options.Prefix + "EventBus", options.Prefix + "EventBus:exchange",
                            options.Prefix + "EventBus:key");
                    })
                }
            ];
        }), typeof(RabbitMQEventBus<>).Assembly);

        return services;
    }
}