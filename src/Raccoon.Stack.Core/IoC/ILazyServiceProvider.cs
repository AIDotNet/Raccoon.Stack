using Microsoft.Extensions.DependencyInjection;

namespace Raccoon.Stack.Core.IoC;

/// <summary>
/// Lazy服务商
/// </summary>
public interface ILazyServiceProvider
#if NET8_0
    : IKeyedServiceProvider
#endif
{
    /// <summary>
    /// 从Lazy服务商获取服务
    /// </summary>
    /// <param name="valueFactory">如果服务不存在，则构建</param>
    /// <typeparam name="T">服务类型</typeparam>
    /// <returns></returns>
    T GetService<T>(Func<IServiceProvider, object> valueFactory) where T : class;

    /// <summary>
    /// 从Lazy服务商获取服务
    /// </summary>
    /// <param name="serviceType">服务类型</param>
    /// <param name="valueFactory">如果服务不存在，则构建</param>
    /// <returns></returns>
    object GetService(Type serviceType, Func<IServiceProvider, object> valueFactory);

    /// <summary>
    /// 从Lazy服务商获取服务，如果服务不存在则抛出异常
    /// </summary>
    /// <typeparam name="T">服务类型</typeparam>
    /// <returns></returns>
    T GetRequiredService<T>() where T : class;

    /// <summary>
    /// 从Lazy服务商获取服务，如果服务不存在则抛出异常
    /// </summary>
    /// <returns></returns>
    /// <exception cref="T:System.InvalidOperationException">There is no service of type <paramref name="serviceType" />.</exception>
    object GetRequiredService(Type serviceType);
}