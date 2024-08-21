using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Raccoon.Stack.EntityFrameworkCore;

public class RaccoonDbContextBuilder : IRaccoonDbContextBuilder
{
    public IServiceCollection Services { get; }

    public Type DbContextType { get; }

    public Action<IServiceProvider, DbContextOptionsBuilder>? Builder { get; set; }

    public List<Action<DbContextOptionsBuilder>> DbContextOptionsBuilders { get; } = new();

    public bool EnableSoftDelete { get; set; }

    public RaccoonDbContextBuilder(IServiceCollection services, Type dbContextType)
    {
        Services = services;
        DbContextType = dbContextType;
    }
}