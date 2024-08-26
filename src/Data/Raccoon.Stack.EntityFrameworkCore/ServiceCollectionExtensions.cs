using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Raccoon.Stack.Data;
using Raccoon.Stack.Data.Concurrency;
using Raccoon.Stack.Data.Exceptions;
using Raccoon.Stack.EntityFrameworkCore.Extensions;
using Raccoon.Stack.EntityFrameworkCore.Filters;

namespace Raccoon.Stack.EntityFrameworkCore;

#pragma warning disable S1135
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRaccoonDbContext<TDbContextImplementation>(
        this IServiceCollection services,
        Action<RaccoonDbContextBuilder>? optionsBuilder = null,
        ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
        ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
        where TDbContextImplementation : DefaultRaccoonDbContext, IRaccoonDbContext
        => services
            .AddDbContext<TDbContextImplementation>(contextLifetime, optionsLifetime)
            .AddCoreServices<TDbContextImplementation>(optionsBuilder, contextLifetime, optionsLifetime);


    private static IServiceCollection AddCoreServices<TDbContextImplementation>(
        this IServiceCollection services,
        Action<RaccoonDbContextBuilder>? optionsBuilder,
        ServiceLifetime contextLifetime,
        ServiceLifetime optionsLifetime)
        where TDbContextImplementation : DefaultRaccoonDbContext, IRaccoonDbContext
    {
#if (NET8_0_OR_GREATER)
        if (services.Any(service => service.IsKeyedService == false && service.ImplementationType == typeof(RaccoonDbContextProvider<TDbContextImplementation>)))
            return services;
#else
        if (services.Any(service =>
                service.ImplementationType == typeof(RaccoonDbContextProvider<TDbContextImplementation>)))
            return services;
#endif

        services.AddSingleton<RaccoonDbContextProvider<TDbContextImplementation>>();

        services.Replace(new ServiceDescriptor(typeof(TDbContextImplementation), serviceProvider =>
        {
            var dbContext = DbContextExtensions.CreateDbContext<TDbContextImplementation>(serviceProvider);
            RaccoonArgumentException.ThrowIfNull(dbContext);

            dbContext.TryInitializeRaccoonDbContextOptions(serviceProvider
                .GetService<RaccoonDbContextOptions<TDbContextImplementation>>());
            return dbContext;
        }, contextLifetime));
        services.TryAddConfigure<ConnectionStrings>();

        RaccoonDbContextBuilder RaccoonBuilder = new(services, typeof(TDbContextImplementation));
        optionsBuilder?.Invoke(RaccoonBuilder);
        return services.AddCoreServices<TDbContextImplementation>((serviceProvider, efDbContextOptionsBuilder) =>
        {
            if (RaccoonBuilder.Builder != null)
            {
                efDbContextOptionsBuilder.DbContextOptionsBuilder.UseApplicationServiceProvider(serviceProvider);
                efDbContextOptionsBuilder.DbContextOptionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior
                    .NoTracking);
                RaccoonBuilder.Builder.Invoke(serviceProvider, efDbContextOptionsBuilder.DbContextOptionsBuilder);

                foreach (var dbContextOptionsBuilder in RaccoonBuilder.DbContextOptionsBuilders)
                {
                    dbContextOptionsBuilder.Invoke(efDbContextOptionsBuilder.DbContextOptionsBuilder);
                }
            }
        }, RaccoonBuilder.EnableSoftDelete, optionsLifetime);
    }

    private static IServiceCollection AddCoreServices<TDbContextImplementation>(
        this IServiceCollection services,
        Action<IServiceProvider, RaccoonDbContextOptionsBuilder>? optionsBuilder,
        bool enableSoftDelete,
        ServiceLifetime optionsLifetime)
        where TDbContextImplementation : DefaultRaccoonDbContext, IRaccoonDbContext
    {
        RaccoonApp.TrySetServiceCollection(services);

        services.TryAddSingleton<IConcurrencyStampProvider, DefaultConcurrencyStampProvider>();

        return services
            .AddRaccoonDbContextOptions<TDbContextImplementation>(optionsBuilder, enableSoftDelete, optionsLifetime)
            .AddFilter<TDbContextImplementation>(optionsLifetime);
    }

    private static IServiceCollection AddFilter<TDbContextImplementation>(
        this IServiceCollection services,
        ServiceLifetime optionsLifetime)
        where TDbContextImplementation : DefaultRaccoonDbContext, IRaccoonDbContext
    {
        services.Add(new ServiceDescriptor(typeof(ISaveChangesFilter<TDbContextImplementation>),
            DbContextExtensions.CreateSaveChangesFilter<TDbContextImplementation>, optionsLifetime));
        services.Add(new ServiceDescriptor(typeof(ISaveChangesFilter<TDbContextImplementation>),
            DbContextExtensions.CreateSoftDeleteSaveChangesFilter<TDbContextImplementation>, optionsLifetime));
        return services;
    }

    private static IServiceCollection AddRaccoonDbContextOptions<TDbContextImplementation>(
        this IServiceCollection services,
        Action<IServiceProvider, RaccoonDbContextOptionsBuilder>? optionsBuilder,
        bool enableSoftDelete,
        ServiceLifetime optionsLifetime)
        where TDbContextImplementation : DbContext, IRaccoonDbContext
    {
        services.TryAdd(
            new ServiceDescriptor(
                typeof(RaccoonDbContextOptions<TDbContextImplementation>),
                serviceProvider =>
                    CreateRaccoonDbContextOptions<TDbContextImplementation>(serviceProvider, optionsBuilder,
                        enableSoftDelete),
                optionsLifetime));

        services.TryAdd(
            new ServiceDescriptor(
                typeof(RaccoonDbContextOptions),
                serviceProvider =>
                    serviceProvider.GetRequiredService<RaccoonDbContextOptions<TDbContextImplementation>>(),
                optionsLifetime));
        return services;
    }

    private static RaccoonDbContextOptions<TDbContextImplementation> CreateRaccoonDbContextOptions<
        TDbContextImplementation>(
        IServiceProvider serviceProvider,
        Action<IServiceProvider, RaccoonDbContextOptionsBuilder>? optionsBuilder,
        bool enableSoftDelete)
        where TDbContextImplementation : DbContext, IRaccoonDbContext
    {
        var RaccoonDbContextOptionsBuilder =
            new RaccoonDbContextOptionsBuilder<TDbContextImplementation>(serviceProvider, enableSoftDelete);
        optionsBuilder?.Invoke(serviceProvider, RaccoonDbContextOptionsBuilder);

        return RaccoonDbContextOptionsBuilder.RaccoonOptions;
    }


    private static IServiceCollection TryAddConfigure<TOptions>(
        this IServiceCollection services)
        where TOptions : class
        => services.AddConfigure<TOptions>(ConnectionStrings.DEFAULT_SECTION, isRoot: false);

#pragma warning disable S2326
#pragma warning disable S2094
    private sealed class RaccoonDbContextProvider<TDbContext>
    {
    }
#pragma warning restore S2094
#pragma warning restore S2326
}