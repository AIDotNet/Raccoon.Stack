using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Raccoon.Stack.Configuration;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Only consider using MasaConfiguration and database configuration using local configuration
    /// When using MasaConfiguration and the database configuration is stored in ConfigurationApi, you need to specify the mapping relationship in Configuration by yourself
    /// </summary>
    /// <param name="services"></param>
    /// <param name="sectionName"></param>
    /// <param name="name"></param>
    /// <param name="isRoot"></param>
    /// <typeparam name="TOptions"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddConfigure<TOptions>(
        this IServiceCollection services,
        string sectionName,
        string? name = null,
        bool isRoot = false)
        where TOptions : class
    {
        services.AddOptions();
        var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetService<IConfiguration>();
        var configurationSection = configuration.GetSection(string.IsNullOrWhiteSpace(sectionName) ? "" : sectionName);
        if (!configurationSection.Exists())
            return services;

        services.Configure<TOptions>(name, isRoot ? configuration : configurationSection);
        return services;
    }

}