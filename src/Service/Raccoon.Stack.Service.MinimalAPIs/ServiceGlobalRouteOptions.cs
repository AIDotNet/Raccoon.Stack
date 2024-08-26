using System.Globalization;
using System.Reflection;
using Raccoon.Stack.Data;

namespace Raccoon.Stack.Service.MinimalAPIs;

public class ServiceGlobalRouteOptions : ServiceRouteOptions
{
    public IEnumerable<Assembly> Assemblies { get; set; }

    public Action<RouteHandlerBuilder>? RouteHandlerBuilder { get; set; }

    internal PluralizationService Pluralization { get; set; }

    public ServiceGlobalRouteOptions()
    {
        DisableAutoMapRoute = false;
        Prefix = "api";
        Version = "v1";
        AutoAppendId = true;
        PluralizeServiceName = true;
        GetPrefixes = new List<string> { "Get", "Select", "Find" };
        PostPrefixes = new List<string> { "Post", "Add", "Upsert", "Create", "Insert" };
        PutPrefixes = new List<string> { "Put", "Update", "Modify" };
        DeletePrefixes = new List<string> { "Delete", "Remove" };
        DisableTrimMethodPrefix = false;
        Assemblies = RaccoonApp.GetAssemblies();
        Pluralization = PluralizationService.CreateService(CultureInfo.CreateSpecificCulture("en"));
        EnableProperty = false;
    }
}
