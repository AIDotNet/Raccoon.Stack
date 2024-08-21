using Microsoft.EntityFrameworkCore;

namespace Raccoon.Stack.EntityFrameworkCore;

public partial class RaccoonDbContextOptionsBuilder
{
    internal IServiceProvider? ServiceProvider { get; }

    internal bool EnableSoftDelete { get; }

    public DbContextOptionsBuilder DbContextOptionsBuilder { get; }

    public Type DbContextType { get; }

    public RaccoonDbContextOptionsBuilder(RaccoonDbContextOptions RaccoonDbContextOptions, Func<DbContextOptionsBuilder>? configure = null)
    {
        ServiceProvider = RaccoonDbContextOptions.ServiceProvider;
        EnableSoftDelete = RaccoonDbContextOptions.EnableSoftDelete;
        DbContextType = RaccoonDbContextOptions.DbContextType;
        DbContextOptionsBuilder = configure == null ? new DbContextOptionsBuilder(RaccoonDbContextOptions) : configure.Invoke();
    }

    public RaccoonDbContextOptionsBuilder(DbContextOptionsBuilder dbContextOptionsBuilder, RaccoonDbContextOptions RaccoonDbContextOptions)
        : this(RaccoonDbContextOptions, () => dbContextOptionsBuilder)
    {
    }
}
