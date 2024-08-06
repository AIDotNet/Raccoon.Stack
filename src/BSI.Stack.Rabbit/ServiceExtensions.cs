using System.Reflection;
using BSI.Stack.Rabbit.Handler;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BSI.Stack.Rabbit;

public static class ServiceExtensions
{
    /// <summary>
    /// 注册rabbit hanlders
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <param name="handlers"></param>
    /// <returns></returns>
    public static IServiceCollection AddRabbitBoot(this IServiceCollection services, Action<RabbitOptions> options,
        Assembly handlers)
    {
        services.AddRabbitClient<RabbitClientBus>(options);

        services.AddHostedService<RabbitBoot>();
        services.AddScoped<IRabbitEventBus, RabbitEventBus>();
        services.Scan(scan => scan.FromAssemblies(handlers)
            .AddClasses(classes => classes.AssignableToAny(typeof(IRabbitHandler)))
            .AsSelf()
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }

    /// <summary>
    /// 注册rabbit客户端
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddRabbitClient<T>(this IServiceCollection services, Action<RabbitOptions> options)
        where T : RabbitClient
    {
        var name = typeof(T).Name;
        services.Configure(name, options);

        services.AddSingleton<T>((IServiceProvider sp) =>
        {
            var log = sp.GetRequiredService<ILogger<T>>();
            var opt = sp.GetRequiredService<IOptionsMonitor<RabbitOptions>>();
            return ActivatorUtilities.CreateInstance<T>(sp, log, opt.Get(name));
        });
        services.AddSingleton<RabbitClient>(sp => sp.GetService<T>());

        return services;
    }
}