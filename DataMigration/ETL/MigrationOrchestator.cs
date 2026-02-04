using DataMigration.Data;
using Microsoft.EntityFrameworkCore;
using TodoSeUsa.Infrastructure.Data;
using static DataMigration.ETL.DTOs.ExtractionDTOs;

namespace DataMigration.ETL;

public class MigrationOrchestrator
{
    private readonly OldDbContext _old;
    private readonly ApplicationDbContext _new;

    public MigrationOrchestrator(OldDbContext oldDb, ApplicationDbContext newDb)
    {
        _old = oldDb;
        _new = newDb;
    }

    public async Task RunAsync()
    {
        var oldClients = await GetActiveClientsAsync();
        var personMap = oldClients.ToDictionary(c => c.ClientId, Transforms.ToPerson);
        await Loader.BulkInsertAsync(_new, personMap.Values);

        var providerMap = personMap.ToDictionary(
            kv => kv.Key,
            kv => Transforms.ToProvider(kv.Value)
        );
        await Loader.BulkInsertAsync(_new, providerMap.Values);

        var oldBills = await GetActiveBillsAsync(oldClients.Select(c => c.ClientId));
        var consignmentMap = oldBills.ToDictionary(
            b => b.BillId,
            b => Transforms.ToConsignment(b, providerMap[b.ClientId].Id)
        );
        await Loader.BulkInsertAsync(_new, consignmentMap.Values);

        var oldProducts = await GetAvailableProductsAsync(oldBills.Select(b => b.BillId));
        var productMap = oldProducts.ToDictionary(
            p => p.ProductId,
            p => Transforms.ToProduct(p, consignmentMap[p.BillId].Id)
        );
        await Loader.BulkInsertAsync(_new, productMap.Values);
    }

    public Task<List<OldClientRow>> GetActiveClientsAsync()
    {
        return _old.Client
            .Where(c => c.Active)
            .ToListAsync();
    }

    public Task<List<OldBillRow>> GetActiveBillsAsync(IEnumerable<int> activeClientIds)
    {
        return _old.Bill
            .Where(b => b.Active && !b.Closed && activeClientIds.Contains(b.ClientId))
            .ToListAsync();
    }

    public Task<List<OldProductRow>> GetAvailableProductsAsync(IEnumerable<int> activeBillIds)
    {
        return _old.Product
            .Where(p => p.Active && (!p.Sold || p.SaleId == null) && activeBillIds.Contains(p.BillId))
            .ToListAsync();
    }
}