using MessagePack;
using MessagePack.Resolvers;
using Microsoft.Extensions.DependencyInjection;

namespace Raccoon.Stack.Codec;

public static class ServiceExtensions
{
    /// <summary>
    /// Add MessagePack serialization
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IServiceCollection AddMessagePack(this IServiceCollection services,
        Action<MessagePackSerializerOptions>? configure)
    {
        var options = MessagePackSerializer.DefaultOptions.WithResolver(
            CompositeResolver.Create(StandardResolver.Instance, ContractlessStandardResolver.Instance));
        configure?.Invoke(options);
        services.AddSingleton(options);
        services.AddSingleton<ISerialization, MessagePackSerialization>();
        return services;
    }
}