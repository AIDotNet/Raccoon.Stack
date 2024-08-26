using Microsoft.Extensions.Options;
using Raccoon.Stack.Service.MinimalAPIs;

namespace Microsoft.AspNetCore.Builder;

public static class WebApplicationExtensions
{
    public static void MapMasaMinimalAPIs(this WebApplication webApplication)
    {
        GlobalMinimalApiOptions.WebApplication = webApplication;

        var serviceMapOptions = webApplication.Services.GetRequiredService<IOptions<ServiceGlobalRouteOptions>>().Value;
        foreach (var serviceType in GlobalMinimalApiOptions.ServiceTypes)
        {
            var serviceInstance = (MinimalAPIBase)webApplication.Services.GetRequiredService(serviceType);
            if (serviceInstance.RouteOptions.DisableAutoMapRoute ?? serviceMapOptions.DisableAutoMapRoute ?? false)
                continue;

            serviceInstance.AutoMapRoute(serviceMapOptions, serviceMapOptions.Pluralization);
        }
    }
}