using System.Linq.Expressions;

namespace Raccoon.Stack.Core.Linq;

public static class QueryableExtensions
{
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition,
        Expression<Func<T, bool>> predicate)
    {
        return condition
            ? query.Where(predicate)
            : query;
    }

    public static TQueryable WhereIf<T, TQueryable>(this TQueryable query, bool condition,
        Expression<Func<T, bool>> predicate)
        where TQueryable : IQueryable<T>
    {
        return condition
            ? (TQueryable)query.Where(predicate)
            : query;
    }

    public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition,
        Expression<Func<T, int, bool>> predicate)
    {
        return condition
            ? query.Where(predicate)
            : query;
    }

    public static TQueryable WhereIf<T, TQueryable>(this TQueryable query, bool condition,
        Expression<Func<T, int, bool>> predicate)
        where TQueryable : IQueryable<T>
    {
        return condition
            ? (TQueryable)query.Where(predicate)
            : query;
    }
}