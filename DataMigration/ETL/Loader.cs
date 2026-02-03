using Microsoft.EntityFrameworkCore;

namespace DataMigration.ETL;

public static class Loader
{
    public static async Task BulkInsertAsync<TEntity>(
        DbContext context,
        IEnumerable<TEntity> items
    ) where TEntity : class
    {
        var list = items.ToList();
        if (list.Count == 0)
            return;

        using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            await context.Set<TEntity>().AddRangeAsync(list);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
