using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Raccoon.Stack.Authentication.Identity;
using Raccoon.Stack.Data;
using Raccoon.Stack.Data.Exceptions;
using Raccoon.Stack.Data.Isolation.Options;
using Raccoon.Stack.Ddd.Domain.Options;
using Raccoon.Stack.EntityFrameworkCore.Filters;
using Raccoon.Stack.EntityFrameworkCore.Isolation;
using Raccoon.Stack.Utils.Caching;

namespace Raccoon.Stack.EntityFrameworkCore.Extensions;

internal static class DbContextExtensions
{
    #region CreateDbContext

    private static readonly MemoryCache<Type, List<Type>> ParameterTypeData = new();

    public static TDbContextImplementation? CreateDbContext<TDbContextImplementation>(IServiceProvider serviceProvider)
        where TDbContextImplementation : DefaultRaccoonDbContext, IRaccoonDbContext
    {
        var parameterTypes = ParameterTypeData.GetOrAdd(typeof(TDbContextImplementation), type =>
        {
            var constructorInfo = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                .MaxBy(c => c.GetParameters().Length);
            RaccoonArgumentException.ThrowIfNull(constructorInfo);
            return constructorInfo.GetParameters().Select(p => p.ParameterType).ToList();
        });
        if (parameterTypes.Count > 0)
        {
            return Activator.CreateInstance(typeof(TDbContextImplementation), parameterTypes.Select(serviceProvider.GetService).ToArray())
                as TDbContextImplementation;
        }

        return Activator.CreateInstance(typeof(TDbContextImplementation)) as TDbContextImplementation;
    }

    #endregion

    #region Filter

    #region IsolationFilter

    private static MemoryCache<Type, object> _emptyIsolationSaveChangesFilter = new();
    private static MemoryCache<Type, Type?> _isolationSaveChangesFilterTypeData = new();

    public static object CreateIsolationSaveChangesFilter<TDbContextImplementation>(
        IServiceProvider serviceProvider)
        where TDbContextImplementation : DefaultRaccoonDbContext, IRaccoonDbContext
    {
        var genericType = _isolationSaveChangesFilterTypeData.GetOrAdd(typeof(TDbContextImplementation), type =>
        {
            var isolationOptions = serviceProvider.GetService<IOptions<IsolationOptions>>();
            if (isolationOptions == null || !isolationOptions.Value.Enable)
                return null;

            return typeof(IsolationSaveChangesFilter<,>).MakeGenericType(typeof(TDbContextImplementation),
                isolationOptions.Value.MultiTenantIdType);
        });
        if (genericType == null)
        {
            return _emptyIsolationSaveChangesFilter.GetOrAdd(typeof(TDbContextImplementation),
                type => new EmptySaveFilter<TDbContextImplementation>());
        }

        var isolationSaveChangesFilter = Activator.CreateInstance(genericType, serviceProvider);
        return (isolationSaveChangesFilter as ISaveChangesFilter<TDbContextImplementation>)!;
    }

    #endregion

    #region SaveChangeFilter

    private static MemoryCache<Type, Type> _saveChangesFilterTypeData = new();

    public static object CreateSaveChangesFilter<TDbContextImplementation>(
        IServiceProvider serviceProvider)
        where TDbContextImplementation : DefaultRaccoonDbContext, IRaccoonDbContext
    {
        var saveChangeFilterType = _saveChangesFilterTypeData.GetOrAdd(typeof(TDbContextImplementation), type =>
        {
            var userIdType = serviceProvider.GetService<IOptions<AuditEntityOptions>>()?.Value.UserIdType ?? typeof(Guid);
            return typeof(SaveChangeFilter<,>).MakeGenericType(type, userIdType);
        });
        return Activator.CreateInstance(saveChangeFilterType,
            serviceProvider.GetService<IUserContext>(),
            serviceProvider.GetService<ITypeAndDefaultValueProvider>(),
            serviceProvider.GetService<ITypeConvertProvider>())!;
    }

    #endregion

    #region SoftDelete SaveChangesFilter

    private static MemoryCache<Type, Type> _softDeleteSaveChangesFilterTypeData = new();

    public static object CreateSoftDeleteSaveChangesFilter<TDbContextImplementation>(
        IServiceProvider serviceProvider)
        where TDbContextImplementation : DefaultRaccoonDbContext, IRaccoonDbContext
    {
        var softDeleteSaveChangesFilterType = _softDeleteSaveChangesFilterTypeData.GetOrAdd(typeof(TDbContextImplementation), type =>
        {
            var userIdType = serviceProvider.GetService<IOptions<AuditEntityOptions>>()?.Value.UserIdType ?? typeof(Guid);
            return typeof(SoftDeleteSaveChangesFilter<,>).MakeGenericType(type, userIdType);
        });
        return Activator.CreateInstance(
            softDeleteSaveChangesFilterType,
            serviceProvider.GetRequiredService<RaccoonDbContextOptions<TDbContextImplementation>>(),
            serviceProvider.GetRequiredService<TDbContextImplementation>(),
            serviceProvider.GetService<IUserContext>(),
            serviceProvider.GetService<ITypeConvertProvider>())!;
    }

    #endregion

    #endregion

    /// <summary>
    /// Initialize cache data, only for testing
    /// </summary>
    internal static void InitializeCacheData()
    {
        _emptyIsolationSaveChangesFilter = new();
        _isolationSaveChangesFilterTypeData = new();
        _saveChangesFilterTypeData = new();
        _softDeleteSaveChangesFilterTypeData = new();
    }
}