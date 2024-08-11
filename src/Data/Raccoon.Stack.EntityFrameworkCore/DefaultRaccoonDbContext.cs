using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Raccoon.Stack.Core.Extensions;
using Raccoon.Stack.Data;
using Raccoon.Stack.Data.Concurrency;
using Raccoon.Stack.Data.Contracts.DataFiltering;
using Raccoon.Stack.Data.Exceptions;
using Raccoon.Stack.Data.Isolation.MultiEnvironment;
using Raccoon.Stack.Data.Isolation.MultiTenant;
using Raccoon.Stack.EntityFrameworkCore.Extensions;

namespace Raccoon.Stack.EntityFrameworkCore;


/// <summary>
/// This is an internal API backing the MasaDbContext infrastructure and is not subject to the same compatibility standards as the public API. It may be changed or removed without notice
/// </summary>
public class DefaultRaccoonDbContext : DbContext, IRaccoonDbContext
{
    private bool _initialized;

    private IDataFilter? _dataFilter;

    protected IDataFilter? DataFilter
    {
        get
        {
            TryInitialize();
            return _dataFilter;
        }
    }

    protected RaccoonDbContextOptions? Options { get; private set; }

    private IConcurrencyStampProvider? _concurrencyStampProvider;

    public IConcurrencyStampProvider? ConcurrencyStampProvider
    {
        get
        {
            TryInitialize();
            return _concurrencyStampProvider;
        }
    }

    private IMultiEnvironmentContext? EnvironmentContext => Options?.ServiceProvider?.GetService<IMultiEnvironmentContext>();

    protected IMultiTenantContext? TenantContext => Options?.ServiceProvider?.GetService<IMultiTenantContext>();

    protected virtual bool IsEnvironmentFilterEnabled => DataFilter?.IsEnabled<IMultiEnvironment>() ?? false;

    protected virtual bool IsTenantFilterEnabled => DataFilter?.IsEnabled<IMultiTenant<Guid>>() ?? false;

    protected DefaultRaccoonDbContext()
    {
    }

    public DefaultRaccoonDbContext(RaccoonDbContextOptions options) : base(options)
    {
        Options = options;
    }

    internal void TryInitializeRaccoonDbContextOptions(RaccoonDbContextOptions? options)
    {
        if (options is { IsInitialize: false })
            Options = options;

        try
        {
            _ = base.ChangeTracker;
        }
        catch (InvalidOperationException ex)
        {
            ILogger? logger = null;
            if (options != null)
            {
                var loggerType = typeof(ILogger<>).MakeGenericType(options.ContextType);
                logger = options.ServiceProvider?.GetService(loggerType) as ILogger;
            }

            logger ??= RaccoonApp.GetService<ILogger<RaccoonDbContext>>();
            logger?.LogDebug(ex, "Error generating data context");

            if (ex.Message.Contains("overriding the 'DbContext.OnConfiguring'"))
                throw new InvalidOperationException("No database provider has been configured for this DbContext. A provider can be configured by overriding the 'MasaDbContext.OnConfiguring' method or by using 'AddMasaDbContext' on the application service provider. If 'AddMasaDbContext' is used, then also ensure that your DbContext type accepts a 'MasaDbContextOptions<TContext>' object in its constructor and passes it to the base constructor for DbContext.", ex);
            throw;
        }
    }

    protected virtual void TryInitialize()
    {
        if (!_initialized) Initialize();
    }

    protected virtual void Initialize()
    {
        _dataFilter = Options?.ServiceProvider?.GetService<IDataFilter>();
        _concurrencyStampProvider = Options?.ServiceProvider?.GetRequiredService<IConcurrencyStampProvider>();
        _initialized = true;
    }

    /// <summary>
    /// Automatic filter soft delete data.
    /// When you override this method,you should call base.<see cref="OnModelCreating(ModelBuilder)"/>.
    /// <inheritdoc/>
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected sealed override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingExecuting(modelBuilder);

        OnModelCreatingConfigureGlobalFilters(modelBuilder);

        foreach (var provider in Options!.ModelCreatingProviders)
            provider.Configure(modelBuilder);
    }

    /// <summary>
    /// Use this method instead of OnModelCreating
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected virtual void OnModelCreatingExecuting(ModelBuilder modelBuilder)
    {
    }

#pragma warning disable S3011
    protected virtual void OnModelCreatingConfigureGlobalFilters(ModelBuilder modelBuilder)
    {
        var methodInfo = GetType().GetMethod(nameof(ConfigureGlobalFilters), BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            methodInfo!.MakeGenericMethod(entityType.ClrType).Invoke(this, new object?[]
            {
                modelBuilder, entityType
            });
        }
    }
#pragma warning restore S3011

    protected virtual void ConfigureGlobalFilters<TEntity>(ModelBuilder modelBuilder, IMutableEntityType mutableEntityType)
        where TEntity : class
    {
        if (mutableEntityType.BaseType == null)
        {
            var filterExpression = CreateFilterExpression<TEntity>();
            if (filterExpression != null)
                modelBuilder.Entity<TEntity>().HasQueryFilter(filterExpression);
        }
    }

    protected virtual Expression<Func<TEntity, bool>>? CreateFilterExpression<TEntity>()
        where TEntity : class
    {
        Expression<Func<TEntity, bool>>? expression = null;

        if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
        {
            expression = entity => !IsSoftDeleteFilterEnabled || !EF.Property<bool>(entity, nameof(ISoftDelete.IsDeleted));
        }

        if (typeof(IMultiEnvironment).IsAssignableFrom(typeof(TEntity)) && EnvironmentContext != null)
        {
            Expression<Func<TEntity, bool>> envFilter = entity => !IsEnvironmentFilterEnabled ||
                                                                  EF.Property<string>(entity, nameof(IMultiEnvironment.Environment))
                                                                      .Equals(EnvironmentContext != null
                                                                          ? EnvironmentContext.CurrentEnvironment
                                                                          : default);
            expression = envFilter.And(expression != null, expression);
        }

        expression = CreateMultiTenantFilterExpression(expression);

        return expression;
    }

    protected virtual Expression<Func<TEntity, bool>>? CreateMultiTenantFilterExpression<TEntity>(
        Expression<Func<TEntity, bool>>? expression)
        where TEntity : class
    {
        if (typeof(IMultiTenant<>).IsGenericInterfaceAssignableFrom(typeof(TEntity)) && TenantContext != null)
        {
            string defaultTenantId = Guid.Empty.ToString();
            Expression<Func<TEntity, bool>> tenantFilter = entity => !IsTenantFilterEnabled ||
                                                                     (EF.Property<Guid>(entity, nameof(IMultiTenant<Guid>.TenantId))
                                                                         .ToString())
                                                                     .Equals(TenantContext.CurrentTenant != null
                                                                         ? TenantContext.CurrentTenant.Id
                                                                         : defaultTenantId);

            expression = tenantFilter.And(expression != null, expression);
        }

        return expression;
    }

    protected virtual bool IsSoftDeleteFilterEnabled
        => Options is { EnableSoftDelete: true } && (DataFilter?.IsEnabled<ISoftDelete>() ?? false);

    /// <summary>
    /// Automatic soft delete.
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int SaveChanges() => SaveChanges(true);

    public sealed override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        OnBeforeSaveChanges();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    protected virtual void OnBeforeSaveChanges()
    {
        ChangeTracker.UpdateRowVersion(ConcurrencyStampProvider);
        OnBeforeSaveChangesByFilters();
    }

    protected virtual async Task OnBeforeSaveChangesAsync()
    {
        ChangeTracker.UpdateRowVersion(ConcurrencyStampProvider);
        OnBeforeSaveChangesByFilters();
        
        await Task.CompletedTask.ConfigureAwait(false);
    }

    protected virtual void OnBeforeSaveChangesByFilters()
    {
        foreach (var filter in Options!.SaveChangesFilters)
        {
            try
            {
                filter.OnExecuting(ChangeTracker);
            }
            catch (Exception ex)
            {
                throw new RaccoonException("An error occured when intercept SaveChanges() or SaveChangesAsync()", ex);
            }
        }
    }

    /// <summary>
    /// Automatic soft delete.
    /// <inheritdoc/>
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => SaveChangesAsync(true, cancellationToken);

    /// <summary>
    /// Automatic soft delete.
    /// <inheritdoc/>
    /// </summary>
    /// <param name="acceptAllChangesOnSuccess"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public sealed override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        await OnBeforeSaveChangesAsync();
        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    protected sealed override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (Options == null)
            return;

        var raccoonDbContextOptionsBuilder = new RaccoonDbContextOptionsBuilder(optionsBuilder, Options);
        OnConfiguring(raccoonDbContextOptionsBuilder);
    }

    protected virtual void OnConfiguring(RaccoonDbContextOptionsBuilder optionsBuilder)
    {
    }
}

/// <summary>
/// This is an internal API backing the MasaDbContext infrastructure and is not subject to the same compatibility standards as the public API. It may be changed or removed without notice
/// </summary>
/// <typeparam name="TMultiTenantId"></typeparam>
public class DefaultRaccoonDbContext<TMultiTenantId> : DefaultRaccoonDbContext
    where TMultiTenantId : IComparable
{
    protected override bool IsTenantFilterEnabled => DataFilter?.IsEnabled<IMultiTenant<TMultiTenantId>>() ?? false;

    protected DefaultRaccoonDbContext() : base()
    {
    }

    public DefaultRaccoonDbContext(RaccoonDbContextOptions options) : base(options)
    {
    }

    protected sealed override Expression<Func<TEntity, bool>>? CreateMultiTenantFilterExpression<TEntity>(
        Expression<Func<TEntity, bool>>? expression)
        where TEntity : class
    {
        if (typeof(IMultiTenant<>).IsGenericInterfaceAssignableFrom(typeof(TEntity)) && TenantContext != null)
        {
            string defaultTenantId = default(TMultiTenantId)?.ToString() ?? string.Empty;
            Expression<Func<TEntity, bool>> tenantFilter = entity => !IsTenantFilterEnabled ||
                                                                     (EF.Property<TMultiTenantId>(entity,
                                                                          nameof(IMultiTenant<TMultiTenantId>.TenantId)).ToString() ??
                                                                      string.Empty)
                                                                     .Equals(TenantContext.CurrentTenant != null
                                                                         ? TenantContext.CurrentTenant.Id
                                                                         : defaultTenantId);

            expression = tenantFilter.And(expression != null, expression);
        }

        return expression;
    }
}
