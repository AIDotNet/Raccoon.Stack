namespace Raccoon.Stack.Rabbit;

public class RabbitClientBus : RabbitClient
{
    private readonly IServiceProvider _sp;

    // ReSharper disable once ConvertToPrimaryConstructor
    public RabbitClientBus(IServiceProvider sp, ILogger<RabbitClient> logger, RabbitOptions rabbitOptions) : base(
        logger, rabbitOptions)
    {
        _sp = sp;
    }

    protected override async Task OnReceived(object model, BasicDeliverEventArgs args, ConsumeOptions options)
    {
        using var scope = _sp.CreateScope();
        var bus = scope.ServiceProvider.GetRequiredService<IRabbitEventBus>();
        await bus.Trigger(scope.ServiceProvider, args, options);
    }
}