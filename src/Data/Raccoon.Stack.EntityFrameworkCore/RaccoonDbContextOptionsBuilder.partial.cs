using Microsoft.EntityFrameworkCore;
using Raccoon.Stack.Data;

namespace Raccoon.Stack.EntityFrameworkCore;

public class RaccoonDbContextOptionsBuilder<TDbContext> : RaccoonDbContextOptionsBuilder
    where TDbContext : DbContext, IRaccoonDbContext
{
    public RaccoonDbContextOptions<TDbContext> RaccoonOptions
        => new(ServiceProvider, DbContextOptionsBuilder.Options, EnableSoftDelete);

    public RaccoonDbContextOptionsBuilder(bool enableSoftDelete = false) : this(null, enableSoftDelete)
    {
    }

    public RaccoonDbContextOptionsBuilder(
        IServiceProvider? serviceProvider,
        bool enableSoftDelete = false)
        : base(new RaccoonDbContextOptions<TDbContext>(serviceProvider, new DbContextOptions<TDbContext>(), enableSoftDelete))
    {
    }
}
