using Raccoon.Stack.Core.String;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Raccoon.Stack.Core;
using Raccoon.Stack.Data;
using Raccoon.Stack.Data.Exceptions;
using Raccoon.Stack.Extensions.DotNet;
using Raccoon.Stack.Service.MinimalAPIs.Core;
using Raccoon.Stack.Service.MinimalAPIs.Helpers;

namespace Raccoon.Stack.Service.MinimalAPIs;

public abstract class MinimalAPIBase : IMinimalAPIService
{
    private WebApplication? _webApplication;

    public WebApplication App => _webApplication ??=
        GlobalMinimalApiOptions.WebApplication ?? RaccoonApp.GetRequiredService<WebApplication>();

    public string BaseUri { get; init; }

    public ServiceRouteOptions RouteOptions { get; } = new();

    public string? ServiceName { get; init; }

    /// <summary>
    /// Based on the RouteHandlerBuilder extension, it is used to extend the mapping method, such as
    /// RouteHandlerBuilder = routeHandlerBuilder =>
    /// {
    ///     routeHandlerBuilder.RequireAuthorization("AtLeast21");
    /// };
    /// </summary>
    public Action<RouteHandlerBuilder>? RouteHandlerBuilder { get; init; }

    private bool? _enableProperty;

#pragma warning disable S4136
    protected MinimalAPIBase()
    {
    }

    protected MinimalAPIBase(string baseUri)
    {
        BaseUri = baseUri;
    }
#pragma warning restore S4136

    public TService? GetService<TService>() where TService : class => GetServiceProvider().GetService<TService>();

    public TService GetRequiredService<TService>() where TService : class => GetServiceProvider().GetRequiredService<TService>();

#pragma warning disable CA2208
    protected virtual IServiceProvider GetServiceProvider()
        => RaccoonApp.GetService<IHttpContextAccessor>()?.HttpContext?.RequestServices ??
           throw new RaccoonException("Failed to get ServiceProvider of current request");
#pragma warning restore CA2208

    internal void AutoMapRoute(ServiceGlobalRouteOptions globalOptions, PluralizationService pluralizationService)
    {
        var methodInfos = GetMethodsByAutoMapRoute(GetType(), globalOptions);

        foreach (var method in methodInfos)
        {
            var handler = ServiceBaseHelper.CreateDelegate(method, this);

            string? pattern = null;
            string? httpMethod = null;
            string? methodName = null;
            var attribute = method.GetCustomAttribute<RoutePatternAttribute>();
            if (attribute != null)
            {
                httpMethod = attribute.HttpMethod;
                if (attribute.StartWithBaseUri)
                    methodName = attribute.Pattern;
                else
                    pattern = attribute.Pattern;
            }

            string prefix = string.Empty;

            if (string.IsNullOrWhiteSpace(httpMethod) || string.IsNullOrWhiteSpace(pattern))
            {
                var result = ParseMethod(globalOptions, method.Name);
                httpMethod ??= result.HttpMethod;
                prefix = result.Prefix;
            }

            pattern ??= ServiceBaseHelper.CombineUris(GetBaseUri(globalOptions, pluralizationService),
                methodName ?? GetMethodName(method, prefix, globalOptions));
            var routeHandlerBuilder = MapMethods(globalOptions, pattern, httpMethod, handler);
            (RouteHandlerBuilder ?? globalOptions.RouteHandlerBuilder)?.Invoke(routeHandlerBuilder);
        }
    }

    protected virtual List<MethodInfo> GetMethodsByAutoMapRoute(Type type, ServiceGlobalRouteOptions globalOptions)
    {
        var bindingFlags = BindingFlags.Public | BindingFlags.Instance;
        var methodInfos = type
            .GetMethods(BindingFlags.DeclaredOnly | bindingFlags)
            .Where(methodInfo =>
                methodInfo.CustomAttributes.All(attr => attr.AttributeType != typeof(IgnoreRouteAttribute)))
            .Concat(type.GetMethods(bindingFlags)
                .Where(methodInfo =>
                    methodInfo.CustomAttributes.Any(attr => attr.AttributeType == typeof(RoutePatternAttribute))))
            .Distinct();
        _enableProperty = RouteOptions.EnableProperty ?? globalOptions.EnableProperty ?? false;
        if (!_enableProperty.Value)
        {
            return methodInfos.Where(methodInfo => !methodInfo.IsSpecialName).ToList();
        }

        return methodInfos.Where(methodInfo
            => !methodInfo.IsSpecialName || (methodInfo.IsSpecialName && methodInfo.Name.StartsWith("get_"))).ToList();
    }

    protected virtual string GetBaseUri(ServiceRouteOptions globalOptions, PluralizationService pluralizationService)
    {
        if (!string.IsNullOrWhiteSpace(BaseUri))
            return BaseUri;

        var list = new List<string>()
        {
            RouteOptions.Prefix ?? globalOptions.Prefix ?? string.Empty,
            RouteOptions.Version ?? globalOptions.Version ?? string.Empty,
            ServiceName ??
            GetServiceName(RouteOptions.PluralizeServiceName ?? globalOptions.PluralizeServiceName ?? false
                ? pluralizationService
                : null)
        };

        return string.Join('/', list.Where(x => !string.IsNullOrWhiteSpace(x)).Select(u => u.Trim('/')));
    }

    protected virtual RouteHandlerBuilder MapMethods(ServiceRouteOptions globalOptions, string pattern,
        string? httpMethod, Delegate handler)
    {
        if (!string.IsNullOrWhiteSpace(httpMethod))
            return App.MapMethods(pattern, new[]
            {
                httpMethod
            }, handler);

        var httpMethods = GetDefaultHttpMethods(globalOptions);
        if (httpMethods.Length > 0)
            return App.MapMethods(pattern, httpMethods, handler);

        return App.Map(pattern, handler);
    }

    protected virtual string[] GetDefaultHttpMethods(ServiceRouteOptions globalOptions)
    {
        if (RouteOptions.MapHttpMethodsForUnmatched.Length > 0)
            return RouteOptions.MapHttpMethodsForUnmatched;

        if (globalOptions.MapHttpMethodsForUnmatched.Length > 0)
            return globalOptions.MapHttpMethodsForUnmatched;

        return Array.Empty<string>();
    }

    protected virtual string GetServiceName(PluralizationService? pluralizationService)
    {
        var serviceName = GetType().Name.TrimEnd("Service", StringComparison.OrdinalIgnoreCase);
        if (pluralizationService == null)
            return serviceName;

        return pluralizationService.Pluralize(serviceName);
    }

    [ExcludeFromCodeCoverage]
    protected virtual string GetMethodName(MethodInfo methodInfo, string prefix, ServiceRouteOptions globalOptions)
    {
        var methodName = TrimMethodPrefix(methodInfo.Name);
        if (!(RouteOptions.AutoAppendId ?? globalOptions.AutoAppendId ?? false))
            return ServiceBaseHelper.TrimEndMethodName(methodName);

        var idParameter = methodInfo.GetParameters().FirstOrDefault(p =>
            p.Name!.Equals("id", StringComparison.OrdinalIgnoreCase) &&
            p.GetCustomAttribute<FromBodyAttribute>() == null &&
            p.GetCustomAttribute<FromFormAttribute>() == null &&
            p.GetCustomAttribute<FromHeaderAttribute>() == null &&
            p.GetCustomAttribute<FromQueryAttribute>() == null &&
            p.GetCustomAttribute<FromServicesAttribute>() == null);
        if (idParameter != null)
        {
            var id = idParameter.ParameterType.IsNullableType() || idParameter.HasDefaultValue ? "{id?}" : "{id}";
            return $"{ServiceBaseHelper.TrimEndMethodName(methodName)}/{id}";
        }

        return ServiceBaseHelper.TrimEndMethodName(methodName);

        string TrimMethodPrefix(string name)
        {
            if (RouteOptions.DisableTrimMethodPrefix ?? globalOptions.DisableTrimMethodPrefix ?? false)
                return name;

            return name.Substring(prefix.Length);
        }
    }

    protected virtual (string? HttpMethod, string Prefix) ParseMethod(ServiceRouteOptions globalOptions,
        string methodName)
    {
        var getPrefixes = RouteOptions.GetPrefixes ?? globalOptions.GetPrefixes!;
        if (_enableProperty!.Value)
        {
            getPrefixes.Insert(0, "get_");
        }

        var prefix = ServiceBaseHelper.ParseMethodPrefix(getPrefixes, methodName);
        if (!string.IsNullOrEmpty(prefix))
            return ("GET", prefix);

        prefix = ServiceBaseHelper.ParseMethodPrefix(RouteOptions.PostPrefixes ?? globalOptions.PostPrefixes!,
            methodName);
        if (!string.IsNullOrEmpty(prefix))
            return ("POST", prefix);

        prefix = ServiceBaseHelper.ParseMethodPrefix(RouteOptions.PutPrefixes ?? globalOptions.PutPrefixes!,
            methodName);
        if (!string.IsNullOrEmpty(prefix))
            return ("PUT", prefix);

        prefix = ServiceBaseHelper.ParseMethodPrefix(RouteOptions.DeletePrefixes ?? globalOptions.DeletePrefixes!,
            methodName);
        if (!string.IsNullOrEmpty(prefix))
            return ("DELETE", prefix);

        return (null, string.Empty);
    }
}