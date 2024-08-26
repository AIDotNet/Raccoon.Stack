using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Raccoon.Stack.Data;
using Raccoon.Stack.Data.Exceptions;
using Raccoon.Stack.Data.Uow;
using Raccoon.Stack.EntityFrameworkCore;

namespace Raccoon.Stack.Uow.EntityFrameworkCore;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection UseUoW<TDbContext>(
        this IServiceCollection services,
        string paramName,
        Action<RaccoonDbContextBuilder>? optionsBuilder = null,
        bool disableRollbackOnFailure = false,
        bool? useTransaction = null)
        where TDbContext : DefaultRaccoonDbContext, IRaccoonDbContext
    {
        RaccoonArgumentException.ThrowIfNull(services, paramName);

#if (NET8_0_OR_GREATER)
        if (services.Any(service => service.IsKeyedService == false && service.ImplementationType == typeof(UoWProvider)))
            return services;
#else
        if (services.Any(service => service.ImplementationType == typeof(UoWProvider)))
            return services;
#endif

        services.AddSingleton<UoWProvider>();
        services.TryAddScoped<IUnitOfWorkAccessor, UnitOfWorkAccessor>();
        services.TryAddSingleton<IUnitOfWorkManager, UnitOfWorkManager<TDbContext>>();

        services.AddScoped<IUnitOfWork>(serviceProvider => new UnitOfWork<TDbContext>(serviceProvider)
        {
            DisableRollbackOnFailure = disableRollbackOnFailure,
            UseTransaction = useTransaction
        });
        if (services.All(service => service.ServiceType != typeof(RaccoonDbContextOptions<TDbContext>)))
            services.AddRaccoonDbContext<TDbContext>(optionsBuilder);

        services.AddScoped<ITransaction, Transaction>();
        RaccoonApp.TrySetServiceCollection(services);
        return services;
    }

    private sealed class UoWProvider
    {
    }
}