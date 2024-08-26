using System.Reflection;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Raccoon.Stack.Core;
using Raccoon.Stack.Data;

namespace Raccoon.Stack.Service.MinimalAPIs;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add all classes that inherit from ServiceBase to Microsoft.Extensions.DependencyInjection.IServiceCollection
    /// <para>Notice: this method must be last call.</para>
    /// </summary>
    /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service to.</param>
    /// <param name="builder">The Microsoft.AspNetCore.Builder.WebApplicationBuilder.</param>
    /// <param name="assemblies">The assembly collection where the MinimalApi service is located</param>
    /// <returns></returns>
    public static WebApplication AddServices(
        this IServiceCollection services,
        WebApplicationBuilder builder,
        Assembly[]? assemblies = null)
        => services.AddServices(builder, options =>
        {
            if (assemblies != null) options.Assemblies = assemblies;
        });

    public static WebApplication AddServices(
        this IServiceCollection services,
        WebApplicationBuilder builder,
        Action<ServiceGlobalRouteOptions> action)
    {
        bool isMinimalApisType = true;

#if (NET8_0_OR_GREATER)
        isMinimalApisType =
 services.Where(service => service.IsKeyedService == false).All(service => service.ImplementationType != typeof(MinimalApisMarkerService));
#else
        isMinimalApisType = services.All(service => service.ImplementationType != typeof(MinimalApisMarkerService));
#endif

        if (isMinimalApisType)
        {
            services.AddSingleton<MinimalApisMarkerService>();

            services.AddHttpContextAccessor();
            services.Configure(action);

            services.TryAddScoped(sp => services); // Version 1.0 will be removed

            services.AddSingleton(new Lazy<WebApplication>(builder.Build, LazyThreadSafetyMode.ExecutionAndPublication))
                .AddTransient(serviceProvider => serviceProvider.GetRequiredService<Lazy<WebApplication>>().Value);

            RaccoonApp.TrySetServiceCollection(services);

            RaccoonApp.Build(services.BuildServiceProvider());
            var serviceMapOptions = RaccoonApp.GetRequiredService<IOptions<ServiceGlobalRouteOptions>>().Value;
            services.AddServices<MinimalAPIBase>(true, (_, serviceInstance) =>
            {
                var instance = (MinimalAPIBase)serviceInstance;
                if (instance.RouteOptions.DisableAutoMapRoute ?? serviceMapOptions.DisableAutoMapRoute ?? false)
                    return;

                instance.AutoMapRoute(serviceMapOptions, serviceMapOptions.Pluralization);
            }, serviceMapOptions.Assemblies.ToArray());
        }

        var serviceProvider = services.BuildServiceProvider();
        var app = serviceProvider.GetRequiredService<WebApplication>();
        if (RaccoonApp.GetJsonSerializerOptions() == null)
            RaccoonApp.TrySetJsonSerializerOptions(app.Services.GetRequiredService<IOptions<JsonOptions>>().Value
                .SerializerOptions);

        RaccoonApp.Build(app.Services);
        return app;
    }

    public static IServiceCollection AddServices<TService>(this IServiceCollection services,
        bool autoFire,
        Action<Type, object>? action,
        params Assembly[] assemblies)
        => (from type in assemblies.SelectMany(assembly => assembly.GetTypes())
            where !type.IsAbstract && BaseOf<TService>(type)
            select type).AddScoped(services, autoFire, action);

    private static IServiceCollection AddScoped(this IEnumerable<Type> serviceTypes,
        IServiceCollection services,
        bool autoFire,
        Action<Type, object>? action = null)
    {
        foreach (var serviceType in serviceTypes)
        {
            services.AddScoped(serviceType);
        }

        if (autoFire)
        {
            foreach (var serviceType in serviceTypes)
            {
                var service = services.BuildServiceProvider().GetService(serviceType);
                action?.Invoke(serviceType, service);
            }
        }

        return services;
    }


    private static bool BaseOf<T>(Type type)
    {
        if (type.BaseType == typeof(T)) return true;

        return type.BaseType != null && BaseOf<T>(type.BaseType);
    }

    public static IServiceCollection AddRaccoonMinimalAPIs(
        this IServiceCollection services,
        Action<ServiceGlobalRouteOptions>? action = null)
    {
        bool isMinimalApisType = true;

#if (NET8_0_OR_GREATER)
        isMinimalApisType =
 services.Where(service => service.IsKeyedService == false).All(service => service.ImplementationType != typeof(MinimalApisMarkerService));
#else
        isMinimalApisType = services.All(service => service.ImplementationType != typeof(MinimalApisMarkerService));
#endif

        if (isMinimalApisType)
        {
            services.AddSingleton<MinimalApisMarkerService>();

            services.AddHttpContextAccessor();
            if (action == null)
            {
                services.Configure<ServiceGlobalRouteOptions>(_ => { });
            }
            else
            {
                services.Configure(action);
            }

            RaccoonApp.TrySetServiceCollection(services);
            var serviceProvider = services.BuildServiceProvider();
            var serviceMapOptions = serviceProvider.GetRequiredService<IOptions<ServiceGlobalRouteOptions>>().Value;
            var serviceTypes = TypeHelper.GetServiceTypes<MinimalAPIBase>(serviceMapOptions.Assemblies.ToArray());

            GlobalMinimalApiOptions.InitializeService();
            foreach (var serviceType in serviceTypes)
            {
                GlobalMinimalApiOptions.AddService(serviceType);
                services.AddSingleton(serviceType);
            }
        }

        return services;
    }

    private sealed class MinimalApisMarkerService
    {
    }
}