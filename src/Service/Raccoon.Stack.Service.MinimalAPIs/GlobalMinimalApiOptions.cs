namespace Raccoon.Stack.Service.MinimalAPIs;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal static class GlobalMinimalApiOptions
{
#pragma warning disable S2223
    public static WebApplication? WebApplication;
#pragma warning restore S2223
    public static List<Type> ServiceTypes { get; private set; } = new();

    public static void InitializeService()
    {
        ServiceTypes = new List<Type>();
    }

    public static void AddService(Type serviceType)
    {
        if (ServiceTypes.Contains(serviceType))
            return;

        ServiceTypes.Add(serviceType);
    }
}
