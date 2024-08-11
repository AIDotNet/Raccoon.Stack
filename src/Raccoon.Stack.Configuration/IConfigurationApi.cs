using Microsoft.Extensions.Configuration;

namespace Raccoon.Stack.Configuration;

public interface IConfigurationApi
{
    public IConfiguration Get(string appId);
}