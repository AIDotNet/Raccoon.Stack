using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Raccoon.Stack.Authentication.Identity.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRaccoonIdentityCore(
        this IServiceCollection services)
        => services.AddRaccoonIdentityCore();

    public static IServiceCollection AddRaccoonIdentityCore(
        this IServiceCollection services,
        Action<IdentityClaimOptions> configureOptions)
    {
        services.AddSingleton<IdentityProvider>();

        services.Configure(configureOptions);

        return services.AddMasaIdentityModelCore();
    }

    private static IServiceCollection AddMasaIdentityModelCore(
        this IServiceCollection services)
    {
        services.TryAddScoped<DefaultUserContext>(serviceProvider => new DefaultUserContext(
            serviceProvider.GetRequiredService<ICurrentPrincipalAccessor>(),
            serviceProvider.GetRequiredService<IOptionsMonitor<IdentityClaimOptions>>())
        );
        services.TryAddScoped<IUserContext>(serviceProvider => serviceProvider.GetService<DefaultUserContext>()!);
        services.TryAddScoped<IMultiTenantUserContext>(serviceProvider => new DefaultMultiTenantUserContext(
            serviceProvider.GetRequiredService<IUserContext>()
        ));
        services.TryAddScoped<IMultiEnvironmentUserContext>(serviceProvider => new DefaultMultiEnvironmentUserContext(
            serviceProvider.GetRequiredService<IUserContext>()));
        services.TryAddScoped<IIsolatedUserContext>(serviceProvider => new DefaultIsolatedUserContext(
            serviceProvider.GetRequiredService<IUserContext>()));
        return services;
    }

#pragma warning disable S2094
    private sealed class IdentityProvider
    {
    }
#pragma warning restore S2094
}