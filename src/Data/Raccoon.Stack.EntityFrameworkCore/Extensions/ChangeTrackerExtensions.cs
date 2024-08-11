using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Raccoon.Stack.Data.Concurrency;

namespace Raccoon.Stack.EntityFrameworkCore.Extensions;

internal static class ChangeTrackerExtensions
{
    public static void UpdateRowVersion(this ChangeTracker changeTracker, IConcurrencyStampProvider? concurrencyStampProvider)
    {
        if (concurrencyStampProvider == null)
            return;

        var entries = changeTracker.Entries().Where(entry
            => (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.State == EntityState.Deleted) &&
               entry.Entity is IHasConcurrencyStamp);
        foreach (var entity in entries)
        {
            entity.CurrentValues[nameof(IHasConcurrencyStamp.RowVersion)] = concurrencyStampProvider.GetRowVersion();
        }
    }
}
