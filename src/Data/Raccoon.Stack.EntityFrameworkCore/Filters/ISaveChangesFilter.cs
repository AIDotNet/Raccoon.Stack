using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Raccoon.Stack.EntityFrameworkCore.Filters;

public interface ISaveChangesFilter
{
    void OnExecuting(ChangeTracker changeTracker);
}
