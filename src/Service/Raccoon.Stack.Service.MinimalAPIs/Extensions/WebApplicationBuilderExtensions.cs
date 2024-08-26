using System.Reflection;
using Raccoon.Stack.Service.MinimalAPIs;

namespace Microsoft.AspNetCore.Builder;

public static class WebApplicationBuilderExtensions
{
    /// <summary>
    /// Add all classes that inherit from ServiceBase to Microsoft.Extensions.DependencyInjection.IServiceCollection
    /// <para>Notice: this method must be last call.</para>
    /// </summary>
    /// <param name="builder">The Microsoft.AspNetCore.Builder.WebApplicationBuilder.</param>
    /// <param name="assemblies">The assembly collection where the MinimalApi service is located</param>
    /// <returns></returns>
    public static WebApplication AddServices(
        this WebApplicationBuilder builder,
        Assembly[]? assemblies = null)
        => builder.Services.AddServices(builder, assemblies);

    /// <summary>
    /// Add all classes that inherit from ServiceBase to Microsoft.Extensions.DependencyInjection.IServiceCollection
    /// <para>Notice: this method must be last call.</para>
    /// </summary>
    /// <param name="builder">The Microsoft.AspNetCore.Builder.WebApplicationBuilder.</param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static WebApplication AddServices(
        this WebApplicationBuilder builder,
        Action<ServiceGlobalRouteOptions> action)
        => builder.Services.AddServices(builder, action);
}