using Microsoft.Extensions.Configuration;

namespace Raccoon.Stack.Configuration;

public interface IRaccoonConfiguration
{
    public IConfiguration Local { get; }

    public IConfigurationApi ConfigurationApi { get; }
}