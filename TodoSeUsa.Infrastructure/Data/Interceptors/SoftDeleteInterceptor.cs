using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TodoSeUsa.Domain.Interfaces;

namespace TodoSeUsa.Infrastructure.Data.Interceptors;

public sealed class SoftDeleteInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null)
        {
            return base.SavingChangesAsync(
                eventData, result, cancellationToken);
        }

        IEnumerable<EntityEntry<ISoftDelete>> entries = eventData
            .Context
            .ChangeTracker
            .Entries<ISoftDelete>()
            .Where(e => e.State == EntityState.Deleted);

        foreach (EntityEntry<ISoftDelete> softDeletableEntity in entries)
        {
            if (softDeletableEntity is not { State: EntityState.Deleted, Entity: ISoftDelete delete })
            {
                continue;
            }
            softDeletableEntity.State = EntityState.Modified;
            delete.DeletedAt = DateTime.Now;
        }
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}