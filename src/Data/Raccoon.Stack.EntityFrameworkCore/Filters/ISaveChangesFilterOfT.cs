using Microsoft.EntityFrameworkCore;
using Raccoon.Stack.Data;

namespace Raccoon.Stack.EntityFrameworkCore.Filters;

public interface ISaveChangesFilter<TDbContext> : ISaveChangesFilter
    where TDbContext : DbContext, IRaccoonDbContext
{
}