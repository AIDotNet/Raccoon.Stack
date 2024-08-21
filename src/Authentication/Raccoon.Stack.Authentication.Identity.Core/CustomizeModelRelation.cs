using System.Reflection;

namespace Raccoon.Stack.Authentication.Identity.Core;

internal class CustomizeModelRelation(Func<object[], object> func, Dictionary<PropertyInfo, MethodInfo> setter)
{
    /// <summary>
    /// For creating custom user models
    /// </summary>
    public Func<object[], object> Func { get; set; } = func;

    public Dictionary<PropertyInfo, MethodInfo> Setters { get; set; } = setter;
}
