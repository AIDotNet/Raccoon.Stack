using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Raccoon.Stack.Data.Exceptions;

namespace Raccoon.Stack.EntityFrameworkCore.Extensions;

#pragma warning disable S3011
#pragma warning disable CS8603
#pragma warning disable S1135
internal static class DbContextOptionsExtensions
{
    private static readonly Func<DbContextOptions, ImmutableSortedDictionary<Type, (IDbContextOptionsExtension Extension, int Ordinal)>>
        Func = InitializeExtensionsMap();

    static Func<DbContextOptions, ImmutableSortedDictionary<Type, (IDbContextOptionsExtension Extension, int Ordinal)>> InitializeExtensionsMap()
    {
        var property =
            typeof(DbContextOptions).GetProperty("ExtensionsMap", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        RaccoonArgumentException.ThrowIfNull(property);
        var param = Expression.Parameter(typeof(DbContextOptions));
        var body = Expression.Property(param, property);
        var lambda = Expression
            .Lambda<Func<DbContextOptions, ImmutableSortedDictionary<Type, (IDbContextOptionsExtension Extension, int Ordinal)>>>(body,
                param);
        return lambda.Compile();
    }

    public static ImmutableSortedDictionary<Type, (IDbContextOptionsExtension Extension, int Ordinal)> GetExtensionsMap(
        this DbContextOptions dbContextOptions)
    {
        return Func.Invoke(dbContextOptions);
    }
}
#pragma warning restore S1135
#pragma warning restore CS8603
#pragma warning restore S3011
