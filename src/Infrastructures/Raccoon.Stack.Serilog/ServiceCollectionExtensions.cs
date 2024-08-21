using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Raccoon.Stack.Serilog;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Use Serilog as the default logger
    /// </summary>
    /// <param name="hostBuilder">
    /// The host builder to use
    /// </param>
    /// <param name="configuration">
    /// The configuration to read from
    /// </param>
    /// <returns></returns>
    public static IHostBuilder UseRaccoonSerilog(this IHostBuilder hostBuilder, IConfiguration configuration)
    {
        hostBuilder.UseSerilog(new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger());

        return hostBuilder;
    }
}