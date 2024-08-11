using Microsoft.Extensions.DependencyInjection;

namespace Raccoon.Stack.Data;

public class SerializationBuilder
{
    public string Name { get; }

    public IServiceCollection Services { get; }

    public SerializationBuilder(string name, IServiceCollection services)
    {
        Name = name;
        Services = services;
    }
}
