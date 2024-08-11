
namespace Raccoon.Stack.Core.IoC;

public static class ServiceExtensions
{
    public static IServiceCollection AddLazyServiceProvider(this IServiceCollection services)
    {
        services.TryAddScoped<ILazyServiceProvider, LazyServiceProvider>();
        return services;
    }
}