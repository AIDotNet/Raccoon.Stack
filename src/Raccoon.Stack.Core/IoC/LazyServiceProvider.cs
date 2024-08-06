using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace Raccoon.Stack.Core.IoC;

/// <summary>
/// 高效的Lazy服务供应商
/// </summary>
public class LazyServiceProvider : ILazyServiceProvider
{
    protected IServiceProvider ServiceProvider { get; }
    protected ConcurrentDictionary<ServiceIdentifier, Lazy<object>> ServiceCacheDic { get; }

    public LazyServiceProvider(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        ServiceCacheDic = new ConcurrentDictionary<ServiceIdentifier, Lazy<object>>();
        ServiceCacheDic.TryAdd(new ServiceIdentifier(typeof(IServiceProvider)),
            new Lazy<object>(() => serviceProvider));
    }

    public object GetService(Type serviceType)
    {
        return ServiceCacheDic.GetOrAdd(
            new ServiceIdentifier(serviceType),
            _ => new Lazy<object>(() => ServiceProvider.GetService(serviceType))
        ).Value;
    }

    public T GetService<T>(Func<IServiceProvider, object> valueFactory) where T : class
    {
        return GetService(typeof(T), valueFactory) as T;
    }

    public object GetService(Type serviceType, Func<IServiceProvider, object> valueFactory)
    {
        return ServiceCacheDic.GetOrAdd(
            new ServiceIdentifier(serviceType),
            _ => new Lazy<object>(() => valueFactory(ServiceProvider))
        ).Value;
    }

    public T GetRequiredService<T>() where T : class
    {
        return GetRequiredService(typeof(T)) as T;
    }

    public object GetRequiredService(Type serviceType)
    {
        return ServiceCacheDic.GetOrAdd(
            new ServiceIdentifier(serviceType),
            _ => new Lazy<object>(() => ServiceProvider.GetRequiredService(serviceType))
        ).Value!;
    }

#if NET8_0
    public object GetKeyedService(Type serviceType, object serviceKey)
    {
        return ServiceCacheDic.GetOrAdd(
            new ServiceIdentifier(serviceKey, serviceType),
            _ => new Lazy<object>(() =>
            {
                if (ServiceProvider is IKeyedServiceProvider keyedServiceProvider)
                {
                    return keyedServiceProvider.GetKeyedService(serviceType, serviceKey);
                }

                throw new InvalidOperationException("This service provider doesn't support keyed services.");
            })
        ).Value;
    }

    public object GetRequiredKeyedService(Type serviceType, object serviceKey)
    {
        return ServiceCacheDic.GetOrAdd(
            new ServiceIdentifier(serviceKey, serviceType),
            _ => new Lazy<object>(() => ServiceProvider.GetRequiredKeyedService(serviceType, serviceKey))
        ).Value;
    }
#endif
}