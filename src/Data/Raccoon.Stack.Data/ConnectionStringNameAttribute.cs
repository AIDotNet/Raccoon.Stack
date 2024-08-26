using System.Reflection;
using Raccoon.Stack.Data.Options;

namespace Raccoon.Stack.Data;

[AttributeUsage(AttributeTargets.Class)]
public class ConnectionStringNameAttribute : Attribute
{
    public string Name { get; set; }

    public ConnectionStringNameAttribute(string name = "") => Name = name;

    private static readonly List<DbContextNameRelationOptions> DbContextNameRelationOptions = new();

    public static string GetConnStringName<T>() => GetConnStringName(typeof(T));

    public static string GetConnStringName(Type type)
    {
        var options = DbContextNameRelationOptions.FirstOrDefault(c => c.DbContextType == type);
        if (options != null) return options.Name;

        var name = type.GetTypeInfo().GetCustomAttribute<ConnectionStringNameAttribute>()?.Name;
        if (string.IsNullOrWhiteSpace(name)) name = ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME;
        DbContextNameRelationOptions.Add(new DbContextNameRelationOptions(name, type));
        return name!;
    }
}