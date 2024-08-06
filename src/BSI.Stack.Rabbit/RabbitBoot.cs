using Microsoft.Extensions.Hosting;

namespace BSI.Stack.Rabbit;

public class RabbitBoot : IHostedService
{
    private readonly RabbitClientBus _rabbitClient;

    // ReSharper disable once ConvertToPrimaryConstructor
    public RabbitBoot(RabbitClientBus rabbitClient)
    {
        _rabbitClient = rabbitClient;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _rabbitClient.Start();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _rabbitClient.Stop();

        return Task.CompletedTask;
    }
}