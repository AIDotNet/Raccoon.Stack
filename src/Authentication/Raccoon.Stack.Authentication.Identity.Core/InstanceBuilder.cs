using System.Linq.Expressions;
using System.Reflection;

namespace Raccoon.Stack.Authentication.Identity.Core;

internal static class InstanceBuilder
{
    private static readonly Expression[] UnaryExpressions = Array.Empty<UnaryExpression>();

    public static Func<object[], object> CreateInstanceDelegate(ConstructorInfo constructorInfo)
    {
        var parameterParameterExpression = Expression.Parameter(typeof(object[]), "parameters");

        return Expression.Lambda<Func<object[], object>>
        (
            Expression.New(constructorInfo, UnaryExpressions),
            parameterParameterExpression
        ).Compile();
    }

    public static Dictionary<PropertyInfo, MethodInfo> GetPropertyAndMethodInfoRelations(Type type)
    {
        var properties = type.GetProperties();
        var propertyDict = new Dictionary<PropertyInfo, MethodInfo>(properties.Length);
        foreach (var property in properties)
        {
            var setMethod = property.GetSetMethod(property.PropertyType.IsNotPublic);
            if (setMethod != null) propertyDict.Add(property, setMethod);
        }
        return propertyDict;
    }
}
