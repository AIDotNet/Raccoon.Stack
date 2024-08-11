using Raccoon.Stack.Data;

namespace Raccoon.Stack.EntityFrameworkCore;

public class RaccoonDbContext : DefaultRaccoonDbContext<Guid>
{
    protected RaccoonDbContext() : base(new RaccoonDbContextOptions<RaccoonDbContext>())
    {
    }

    protected RaccoonDbContext(RaccoonDbContextOptions options) : base(options)
    {
    }
}

public abstract class RaccoonDbContext<TDbContext> : RaccoonDbContext
    where TDbContext : RaccoonDbContext<TDbContext>, IRaccoonDbContext
{
    protected RaccoonDbContext() : base()
    {
    }

    protected RaccoonDbContext(RaccoonDbContextOptions<TDbContext> options) : base(options)
    {
    }
}

public abstract class RaccoonDbContext<TDbContext, TMultiTenantId> : DefaultRaccoonDbContext<TMultiTenantId>
    where TDbContext : DefaultRaccoonDbContext<TMultiTenantId>, IRaccoonDbContext
    where TMultiTenantId : IComparable
{
    protected RaccoonDbContext() : base()
    {
    }

    protected RaccoonDbContext(RaccoonDbContextOptions<TDbContext> options) : base(options)
    {
    }
}