﻿using Microsoft.Extensions.DependencyInjection;
using Raccoon.Stack.Data;
using Raccoon.Stack.Data.Options;
using Raccoon.Stack.Data.Uow;
using Raccoon.Stack.EntityFrameworkCore;

namespace Raccoon.Stack.Uow.EntityFrameworkCore;

public class UnitOfWorkManager<TDbContext> : IUnitOfWorkManager
    where TDbContext : DefaultRaccoonDbContext, IRaccoonDbContext
{
    private readonly IServiceProvider _serviceProvider;

    public UnitOfWorkManager(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    /// <summary>
    /// Create new DbContext
    /// We create DbContext with lazy loading enabled by default
    /// </summary>
    /// <param name="lazyLoading">Deferred creation of DbContext, easy to specify tenant or environment by yourself, which is very effective for physical isolation</param>
    /// <returns></returns>
    public IUnitOfWork CreateDbContext(bool lazyLoading = true)
    {
        var scope = _serviceProvider.CreateScope();
        if (!lazyLoading)
            scope.ServiceProvider.GetRequiredService<TDbContext>();

        return scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    }

    public IUnitOfWork CreateDbContext(DbContextConnectionStringOptions connectionStringOptions)
    {
        ArgumentNullException.ThrowIfNull(connectionStringOptions, nameof(connectionStringOptions));

        if (string.IsNullOrEmpty(connectionStringOptions.ConnectionString))
            throw new ArgumentException($"Invalid {nameof(connectionStringOptions)}");

        var scope = _serviceProvider.CreateScope();
        var unitOfWorkAccessor = scope.ServiceProvider.GetRequiredService<IUnitOfWorkAccessor>();
        unitOfWorkAccessor.CurrentDbContextOptions.AddConnectionString(
            ConnectionStringNameAttribute.GetConnStringName(typeof(TDbContext)),
            connectionStringOptions.ConnectionString);

        return scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    }
}