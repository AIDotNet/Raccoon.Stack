using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Raccoon.Stack.Configuration;
using Raccoon.Stack.Data.Exceptions;
using Raccoon.Stack.Data.Isolation.Options;

namespace Raccoon.Stack.Data.Isolation;


internal static class ComponentConfigUtils
{
    public static List<IsolationConfigurationOptions<TComponentConfig>> GetComponentConfigs<TComponentConfig>(
        IServiceProvider serviceProvider,
        string name,
        string sectionName)
        where TComponentConfig : class
    {
        var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<IsolationOptions<TComponentConfig>>>().Get(name);
        if (optionsMonitor.Data.Count > 0)
            return optionsMonitor.Data;

        var isolationOptions = serviceProvider.GetRequiredService<IOptions<IsolationOptions>>();
        var rootSectionName = isolationOptions.Value.SectionName;

        var configuration = serviceProvider.GetService<IRaccoonConfiguration>()?.Local ?? serviceProvider.GetService<IConfiguration>();
        RaccoonArgumentException.ThrowIfNull(configuration);
        return configuration
            .GetSection(rootSectionName)
            .GetSection(sectionName)
            .Get<List<IsolationConfigurationOptions<TComponentConfig>>>() ?? new();
    }

    /// <summary>
    /// Get runtime configuration information
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="name"></param>
    /// <param name="sectionName"></param>
    /// <param name="defaultFunc"></param>
    /// <typeparam name="TComponentConfig"></typeparam>
    /// <returns></returns>
    public static TComponentConfig GetComponentConfigByExecute<TComponentConfig>(IServiceProvider serviceProvider,
        string name,
        string sectionName,
        Func<TComponentConfig> defaultFunc) where TComponentConfig : class
    {
        var isolationOptions = serviceProvider.GetRequiredService<IOptions<IsolationOptions>>();
        if (isolationOptions.Value.Enable)
        {
            return serviceProvider
                .GetRequiredService<IIsolationConfigProvider>()
                .GetComponentConfig<TComponentConfig>(sectionName, name) ?? defaultFunc.Invoke();
        }
        return defaultFunc.Invoke();
    }
}
