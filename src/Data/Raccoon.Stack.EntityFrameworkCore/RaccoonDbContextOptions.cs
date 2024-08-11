using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Raccoon.Stack.Data;
using Raccoon.Stack.EntityFrameworkCore.Extensions;
using Raccoon.Stack.EntityFrameworkCore.Filters;

namespace Raccoon.Stack.EntityFrameworkCore;


public abstract class RaccoonDbContextOptions : DbContextOptions
{
    public readonly IServiceProvider? ServiceProvider;

    public abstract IEnumerable<IModelCreatingProvider> ModelCreatingProviders { get; }

    /// <summary>
    /// Can be used to intercept SaveChanges(Async) method
    /// </summary>
    public abstract IEnumerable<ISaveChangesFilter> SaveChangesFilters { get; }

    internal virtual bool IsInitialize => ExtensionsMap.Count > 0;

    public bool EnableSoftDelete { get; }

    internal Type DbContextType { get; }

    protected readonly DbContextOptions OriginOptions;

    private protected RaccoonDbContextOptions(
        IServiceProvider? serviceProvider,
        bool enableSoftDelete,
        Type dbContextType,
        DbContextOptions originOptions)
    {
        ServiceProvider = serviceProvider;
        EnableSoftDelete = enableSoftDelete;
        DbContextType = dbContextType;

        OriginOptions = originOptions;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override Type ContextType => OriginOptions.ContextType;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override bool IsFrozen => OriginOptions.IsFrozen;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override IEnumerable<IDbContextOptionsExtension> Extensions => OriginOptions.Extensions;

    protected override ImmutableSortedDictionary<Type, (IDbContextOptionsExtension Extension, int Ordinal)> ExtensionsMap
        => OriginOptions.GetExtensionsMap();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <typeparam name="TExtension"></typeparam>
    /// <param name="extension"></param>
    /// <returns></returns>
    public override DbContextOptions WithExtension<TExtension>(TExtension extension)
        => OriginOptions.WithExtension(extension);

    public override TExtension? FindExtension<TExtension>() where TExtension : class
        => OriginOptions.FindExtension<TExtension>();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Freeze() => OriginOptions.Freeze();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <typeparam name="TExtension"></typeparam>
    /// <returns></returns>
    public override TExtension GetExtension<TExtension>()
        => OriginOptions.GetExtension<TExtension>();

    public override int GetHashCode()
    {
        var hashCode = new HashCode();

        foreach (var dbContextOptionsExtension in ExtensionsMap)
        {
            hashCode.Add(dbContextOptionsExtension.Key);
            hashCode.Add(dbContextOptionsExtension.Value.Extension.Info.GetServiceProviderHashCode());
        }

        return hashCode.ToHashCode();
    }

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj)
            || (obj is DbContextOptions otherOptions && Equals(otherOptions));
}

public class RaccoonDbContextOptions<TDbContext> : RaccoonDbContextOptions
    where TDbContext : DbContext, IRaccoonDbContext
{
    public RaccoonDbContextOptions() : base(null, false, typeof(TDbContext), new DbContextOptions<TDbContext>())
    {
    }

    public RaccoonDbContextOptions(
        IServiceProvider? serviceProvider,
        DbContextOptions originOptions,
        bool enableSoftDelete) : base(serviceProvider, enableSoftDelete, typeof(TDbContext), originOptions)
    {
    }

    private IEnumerable<IModelCreatingProvider>? _modelCreatingProviders;

    /// <summary>
    /// Can be used to filter data
    /// </summary>
    public override IEnumerable<IModelCreatingProvider> ModelCreatingProviders
        => _modelCreatingProviders ??= ServiceProvider?.GetServices<IModelCreatingProvider>() ?? new List<IModelCreatingProvider>();

    private IEnumerable<ISaveChangesFilter<TDbContext>>? _saveChangesFilters;

    /// <summary>
    /// Can be used to intercept SaveChanges(Async) method
    /// </summary>
    public override IEnumerable<ISaveChangesFilter> SaveChangesFilters
        => _saveChangesFilters ??=
            ServiceProvider?.GetServices<ISaveChangesFilter<TDbContext>>() ?? new List<ISaveChangesFilter<TDbContext>>();
}
