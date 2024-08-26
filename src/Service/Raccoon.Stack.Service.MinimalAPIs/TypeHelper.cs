using System.Reflection;

namespace Raccoon.Stack.Service.MinimalAPIs;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal static class TypeHelper
{
    public static IEnumerable<Type> GetServiceTypes<TService>(params Assembly[] assemblies)
        where TService : class
        => from type in assemblies.SelectMany(assembly => assembly.GetTypes())
            where !type.IsAbstract && BaseOf<TService>(type)
            select type;

    private static bool BaseOf<T>(Type type)
    {
        if (type.BaseType == typeof(T)) return true;

        return type.BaseType != null && BaseOf<T>(type.BaseType);
    }
}
