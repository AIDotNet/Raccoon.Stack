using Microsoft.Extensions.DependencyInjection;

namespace Raccoon.Stack.EntityFrameworkCore;

public interface IRaccoonDbContextBuilder
{
    public bool EnableSoftDelete { get; set; }

    public IServiceCollection Services { get; }
}