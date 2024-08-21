using Microsoft.EntityFrameworkCore.ChangeTracking;
using Raccoon.Stack.Data;

namespace Raccoon.Stack.EntityFrameworkCore.Filters;

public class EmptySaveFilter<TDbContext> : ISaveChangesFilter<TDbContext>
    where TDbContext : DefaultRaccoonDbContext, IRaccoonDbContext
{
    public void OnExecuting(ChangeTracker changeTracker)
    {
        //Empty implementation, no processing required
    }
}