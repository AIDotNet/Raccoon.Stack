using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Raccoon.Stack.Core.Linq;

/// <summary>
/// Expression表达式扩展操作类
/// </summary>
public static class ExpressionExtensions
{
    /// <summary>
    /// 以特定的条件运行组合两个Expression表达式
    /// </summary>
    /// <typeparam name="T">表达式的主实体类型</typeparam>
    /// <param name="first">第一个Expression表达式</param>
    /// <param name="second">要组合的Expression表达式</param>
    /// <param name="merge">组合条件运算方式</param>
    /// <returns>组合后的表达式</returns>
    public static Expression<T> Compose<T>([NotNull] this Expression<T> first, [NotNull] Expression<T> second,
        [NotNull] Func<Expression, Expression, Expression> merge)
    {
        var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] })
            .ToDictionary(p => p.s, p => p.f);
        var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);
        return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
    }

    /// <summary>
    /// 以 Expression.AndAlso 组合两个Expression表达式
    /// </summary>
    /// <typeparam name="T">表达式的主实体类型</typeparam>
    /// <param name="first">第一个Expression表达式</param>
    /// <param name="second">要组合的Expression表达式</param>
    /// <param name="ifExp">判断条件表达式，当此条件为true时，才执行组合</param>
    /// <returns>组合后的表达式</returns>
    public static Expression<Func<T, bool>> And<T>([NotNull] this Expression<Func<T, bool>> first,
        [NotNull] Expression<Func<T, bool>> second, bool ifExp = true)
    {
        return ifExp ? first.Compose(second, Expression.AndAlso) : first;
    }

    /// <summary>
    /// 以 Expression.OrElse 组合两个Expression表达式
    /// </summary>
    /// <typeparam name="T">表达式的主实体类型</typeparam>
    /// <param name="first">第一个Expression表达式</param>
    /// <param name="second">要组合的Expression表达式</param>
    /// <param name="ifExp">判断条件表达式，当此条件为true时，才执行组合</param>
    /// <returns>组合后的表达式</returns>
    public static Expression<Func<T, bool>> Or<T>([NotNull] this Expression<Func<T, bool>> first,
        [NotNull] Expression<Func<T, bool>> second, bool ifExp = true)
    {
        return ifExp ? first.Compose(second, Expression.OrElse) : first;
    }


    private class ParameterRebinder : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> _map;

        private ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            _map = map ?? new();
        }

        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map,
            Expression exp)
        {
            return new ParameterRebinder(map).Visit(exp);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            ParameterExpression replacement;
            if (_map.TryGetValue(node, out replacement))
            {
                node = replacement;
            }

            return base.VisitParameter(node);
        }
    }
}