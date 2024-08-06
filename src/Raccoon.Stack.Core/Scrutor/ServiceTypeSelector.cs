﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Raccoon.Stack.Core.Scrutor;
using Raccoon.Stack.Core.Scrutor.Scrutor;

namespace Scrutor;

internal class ServiceTypeSelector : IServiceTypeSelector, ISelector
{
    public ServiceTypeSelector(IImplementationTypeSelector inner, IEnumerable<Type> types)
    {
        Inner = inner;
        Types = types;
    }

    private IImplementationTypeSelector Inner { get; }

    private IEnumerable<Type> Types { get; }

    private List<ISelector> Selectors { get; } = new();

    private RegistrationStrategy? RegistrationStrategy { get; set; }

    public ILifetimeSelector AsSelf()
    {
        return As(t => new[] { t });
    }

    public ILifetimeSelector As<T>()
    {
        return As(typeof(T));
    }

    public ILifetimeSelector As(params Type[] types)
    {
        Preconditions.NotNull(types, nameof(types));

        return As(types.AsEnumerable());
    }

    public ILifetimeSelector As(IEnumerable<Type> types)
    {
        Preconditions.NotNull(types, nameof(types));

        return AddSelector(Types.Select(t => new TypeMap(t, types)), Enumerable.Empty<TypeFactoryMap>());
    }

    public ILifetimeSelector AsImplementedInterfaces()
    {
        return AsImplementedInterfaces(_ => true);
    }

    public ILifetimeSelector AsImplementedInterfaces(Func<Type, bool> predicate)
    {
        Preconditions.NotNull(predicate, nameof(predicate));

        return As(t => t.GetInterfaces()
            .Where(x => x.HasMatchingGenericArity(t))
            .Select(x => x.GetRegistrationType(t))
            .Where(predicate));
    }

    public ILifetimeSelector AsSelfWithInterfaces()
    {
        IEnumerable<Type> Selector(Type type)
        {
            if (type.IsGenericTypeDefinition)
            {
                // This prevents trying to register open generic types
                // with an ImplementationFactory, which is unsupported.
                return Enumerable.Empty<Type>();
            }

            return type.GetInterfaces()
                .Where(x => x.HasMatchingGenericArity(type))
                .Select(x => x.GetRegistrationType(type));
        }

        return AddSelector(
            Types.Select(t => new TypeMap(t, new[] { t })),
            Types.Select(t => new TypeFactoryMap(x => x.GetRequiredService(t), Selector(t))));
    }

    public ILifetimeSelector AsMatchingInterface()
    {
        return AsMatchingInterface(null);
    }

    public ILifetimeSelector AsMatchingInterface(Action<Type, IImplementationTypeFilter>? action)
    {
        return As(t => t.FindMatchingInterface(action));
    }

    public ILifetimeSelector As(Func<Type, IEnumerable<Type>> selector)
    {
        Preconditions.NotNull(selector, nameof(selector));

        return AddSelector(Types.Select(t => new TypeMap(t, selector(t))), Enumerable.Empty<TypeFactoryMap>());
    }

    public IImplementationTypeSelector UsingAttributes()
    {
        var selector = new AttributeSelector(Types);

        Selectors.Add(selector);

        return this;
    }

    public IServiceTypeSelector UsingRegistrationStrategy(RegistrationStrategy registrationStrategy)
    {
        Preconditions.NotNull(registrationStrategy, nameof(registrationStrategy));

        RegistrationStrategy = registrationStrategy;
        return this;
    }

    #region Chain Methods

    public IImplementationTypeSelector FromCallingAssembly()
    {
        return Inner.FromCallingAssembly();
    }

    public IImplementationTypeSelector FromExecutingAssembly()
    {
        return Inner.FromExecutingAssembly();
    }

    public IImplementationTypeSelector FromEntryAssembly()
    {
        return Inner.FromEntryAssembly();
    }

    public IImplementationTypeSelector FromApplicationDependencies()
    {
        return Inner.FromApplicationDependencies();
    }

    public IImplementationTypeSelector FromApplicationDependencies(Func<Assembly, bool> predicate)
    {
        return Inner.FromApplicationDependencies(predicate);
    }

    public IImplementationTypeSelector FromAssemblyDependencies(Assembly assembly)
    {
        return Inner.FromAssemblyDependencies(assembly);
    }

    public IImplementationTypeSelector FromDependencyContext(DependencyContext context)
    {
        return Inner.FromDependencyContext(context);
    }

    public IImplementationTypeSelector FromDependencyContext(DependencyContext context, Func<Assembly, bool> predicate)
    {
        return Inner.FromDependencyContext(context, predicate);
    }

    public IImplementationTypeSelector FromAssemblyOf<T>()
    {
        return Inner.FromAssemblyOf<T>();
    }

    public IImplementationTypeSelector FromAssembliesOf(params Type[] types)
    {
        return Inner.FromAssembliesOf(types);
    }

    public IImplementationTypeSelector FromAssembliesOf(IEnumerable<Type> types)
    {
        return Inner.FromAssembliesOf(types);
    }

    public IImplementationTypeSelector FromAssemblies(params Assembly[] assemblies)
    {
        return Inner.FromAssemblies(assemblies);
    }

    public IImplementationTypeSelector FromAssemblies(IEnumerable<Assembly> assemblies)
    {
        return Inner.FromAssemblies(assemblies);
    }

    public IServiceTypeSelector AddClasses()
    {
        return Inner.AddClasses();
    }

    public IServiceTypeSelector AddClasses(bool publicOnly)
    {
        return Inner.AddClasses(publicOnly);
    }

    public IServiceTypeSelector AddClasses(Action<IImplementationTypeFilter> action)
    {
        return Inner.AddClasses(action);
    }

    public IServiceTypeSelector AddClasses(Action<IImplementationTypeFilter> action, bool publicOnly)
    {
        return Inner.AddClasses(action, publicOnly);
    }

    #endregion

    internal void PropagateLifetime(ServiceLifetime lifetime)
    {
        foreach (var selector in Selectors.OfType<LifetimeSelector>())
        {
            selector.Lifetime = lifetime;
        }
    }

    void ISelector.Populate(IServiceCollection services, RegistrationStrategy? registrationStrategy)
    {
        if (Selectors.Count == 0)
        {
            AsSelf();
        }

        var strategy = RegistrationStrategy ?? registrationStrategy;

        foreach (var selector in Selectors)
        {
            selector.Populate(services, strategy);
        }
    }

    private ILifetimeSelector AddSelector(IEnumerable<TypeMap> types, IEnumerable<TypeFactoryMap> factories)
    {
        var selector = new LifetimeSelector(this, types, factories);

        Selectors.Add(selector);

        return selector;
    }
}