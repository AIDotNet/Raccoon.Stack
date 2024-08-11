using Microsoft.Extensions.DependencyInjection;

namespace Raccoon.Stack.Core.AutoRaccoon;

public abstract class DecorationStrategy
{
    protected DecorationStrategy(Type serviceType)
    {
        ServiceType = serviceType;
    }

    public Type ServiceType { get; }

    public abstract bool CanDecorate(Type serviceType);

    public abstract Func<IServiceProvider, object> CreateDecorator(Type serviceType);

    internal static DecorationStrategy WithType(Type serviceType, Type decoratorType)
    {
        return Create(serviceType, decoratorType, null);
    }

    internal static DecorationStrategy WithFactory(Type serviceType,
        Func<object, IServiceProvider, object> decoratorFactory)
    {
        return Create(serviceType, null, decoratorFactory);
    }

    protected static Func<IServiceProvider, object> TypeDecorator(Type serviceType, Type decoratorType)
    {
        return serviceProvider =>
        {
            var instanceToDecorate = serviceProvider.GetRequiredService(serviceType);
            return ActivatorUtilities.CreateInstance(serviceProvider, decoratorType, instanceToDecorate);
        };
    }

    protected static Func<IServiceProvider, object> FactoryDecorator(Type decorated,
        Func<object, IServiceProvider, object> decoratorFactory)
    {
        return serviceProvider =>
        {
            var instanceToDecorate = serviceProvider.GetRequiredService(decorated);
            return decoratorFactory(instanceToDecorate, serviceProvider);
        };
    }

    private static DecorationStrategy Create(Type serviceType, Type? decoratorType,
        Func<object, IServiceProvider, object>? decoratorFactory)
    {
        if (serviceType.IsOpenGeneric())
            return new OpenGenericDecorationStrategy(serviceType, decoratorType, decoratorFactory);

        return new ClosedTypeDecorationStrategy(serviceType, decoratorType, decoratorFactory);
    }
}