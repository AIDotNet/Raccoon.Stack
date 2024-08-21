using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Raccoon.Stack.Data;
using Raccoon.Stack.Data.TypeConverts;
using Raccoon.Stack.EntityFrameworkCore;

namespace Raccoon.Stack.Authentication.Identity.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRaccoonIdentityCore(
        this IServiceCollection services,
        Action<JsonSerializerOptions>? configure)
        => services.AddRaccoonIdentityCore(_ => { }, configure);

    public static IServiceCollection AddRaccoonIdentityCore(
        this IServiceCollection services,
        Action<IdentityClaimOptions> configureOptions,
        Action<JsonSerializerOptions>? configure = null)
    {
        services.AddSingleton<IdentityProvider>();

        services.Configure(configureOptions);

        bool isInitialized = false;
        JsonSerializerOptions? jsonSerializerOptions = null;
        return services.AddMasaIdentityModelCore(_ =>
        {
            JsonSerializerOptions? jsonSerializerOptionsTemp = jsonSerializerOptions;
            if (!isInitialized)
            {
                if (configure != null)
                {
                    jsonSerializerOptionsTemp = new JsonSerializerOptions();
                    configure.Invoke(jsonSerializerOptionsTemp);
                }

                jsonSerializerOptions = jsonSerializerOptionsTemp;
                isInitialized = true;
            }

            var deserializer = new DefaultJsonDeserializer(jsonSerializerOptionsTemp);
            var typeConvertProvider = new DefaultTypeConvertProvider(deserializer);
            return typeConvertProvider;
        });
    }

    public static IServiceCollection AddRaccoonIdentityCore(
        this IServiceCollection services,
        Action<IdentityClaimOptions> configureOptions)
    {
        services.AddSingleton<IdentityProvider>();

        services.Configure(configureOptions);

        return services.AddMasaIdentityModelCore(_ =>
            new DefaultTypeConvertProvider(new DefaultJsonDeserializer(new JsonSerializerOptions())));
    }

    private static IServiceCollection AddMasaIdentityModelCore(
        this IServiceCollection services,
        Func<IServiceProvider, ITypeConvertProvider> func)
    {
        services.TryAddScoped<DefaultUserContext>(serviceProvider => new DefaultUserContext(
            func.Invoke(serviceProvider),
            serviceProvider.GetRequiredService<ICurrentPrincipalAccessor>(),
            serviceProvider.GetRequiredService<IOptionsMonitor<IdentityClaimOptions>>())
        );
        services.TryAddScoped<IUserContext>(serviceProvider => serviceProvider.GetService<DefaultUserContext>()!);
        services.TryAddScoped<IMultiTenantUserContext>(serviceProvider => new DefaultMultiTenantUserContext(
            serviceProvider.GetRequiredService<IUserContext>(),
            func.Invoke(serviceProvider)
        ));
        services.TryAddScoped<IMultiEnvironmentUserContext>(serviceProvider => new DefaultMultiEnvironmentUserContext(
            serviceProvider.GetRequiredService<IUserContext>(),
            func.Invoke(serviceProvider)));
        services.TryAddScoped<IIsolatedUserContext>(serviceProvider => new DefaultIsolatedUserContext(
            serviceProvider.GetRequiredService<IUserContext>(),
            func.Invoke(serviceProvider)));
        return services;
    }

#pragma warning disable S2094
    private sealed class IdentityProvider
    {
    }
#pragma warning restore S2094
}