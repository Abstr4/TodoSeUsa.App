using DataMigration.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TodoSeUsa.Domain.Enums;
using TodoSeUsa.Infrastructure.Data;

namespace DataMigration.ETL;

public class MigrationValidator
{
    private readonly OldDbContext _old;
    private readonly ApplicationDbContext _new;
    private readonly ILogger<MigrationValidator> _logger;

    public MigrationValidator(OldDbContext oldDb, ApplicationDbContext newDb, ILogger<MigrationValidator> logger)
    {
        _old = oldDb;
        _new = newDb;
        _logger = logger;
    }

    public async Task ValidateAsync()
    {
        await ValidateClientCountsAsync();
        await ValidateBillCountsAsync();
        await ValidateProductCountsAsync();

        await ValidateProviderLinksAsync();
        await ValidateConsignmentLinksAsync();
        await ValidateProductLinksAsync();

        await ValidateProductRulesAsync();
    }

    private async Task ValidateClientCountsAsync()
    {
        _logger.LogInformation("Starting client count validation...");
        var source = await _old.Client.CountAsync(c => c.Active);
        var target = await _new.Persons.CountAsync();

        _logger.LogInformation($"Source active clients: {source}. Target persons: {target}");

        AssertEqual(source, target, "Active clients");

        _logger.LogInformation("Client count validation passed.\n");
    }

    private async Task ValidateBillCountsAsync()
    {
        _logger.LogInformation("Starting bill count validation...");
        var source = await _old.Bill.CountAsync(b => b.Active && !b.Closed);
        var target = await _new.Consignments.CountAsync();

        _logger.LogInformation($"Souce active bills: {source}. Target consignments: {target}");

        AssertEqual(source, target, "Active bills");

        _logger.LogInformation("Bill count validation passed.\n");
    }

    private async Task ValidateProductCountsAsync()
    {
        _logger.LogInformation("Starting product count validation...");
        var source = await _old.Product.CountAsync(p =>
            p.Active && (!p.Sold || p.SaleId == null)
        );
        var target = await _new.Products.CountAsync();

        _logger.LogInformation($"Source available products: {source}. Target products: {target}");

        AssertEqual(source, target, "Available products");

        _logger.LogInformation("Product count validation passed.\n");
    }

    private async Task ValidateProviderLinksAsync()
    {
        _logger.LogInformation("Starting provider link validation...");
        var broken = await _new.Providers
            .Where(p => !_new.Persons.Any(pe => pe.Id == p.PersonId))
            .AnyAsync();

        AssertFalse(broken, "Providers without Person");

        _logger.LogInformation("There are no providers without associated persons.");
        _logger.LogInformation("Provider link validation passed.\n");
    }

    private async Task ValidateConsignmentLinksAsync()
    {
        _logger.LogInformation("Starting consignment link validation...");
        var broken = await _new.Consignments
            .Where(c => !_new.Providers.Any(p => p.Id == c.ProviderId))
            .AnyAsync();

        AssertFalse(broken, "Consignments without Provider");

        _logger.LogInformation("There are no consignments without associated provider.");
        _logger.LogInformation("Consignment link validation passed.\n");
    }

    private async Task ValidateProductLinksAsync()
    {
        _logger.LogInformation("Starting product link validation...");
        var broken = await _new.Products
            .Where(p => !_new.Consignments.Any(c => c.Id == p.ConsignmentId))
            .AnyAsync();

        AssertFalse(broken, "Products without Consignment");

        _logger.LogInformation("There are no products without associated consignments.");
        _logger.LogInformation("Product link validation passed.\n");
    }

    private async Task ValidateProductRulesAsync()
    {
        _logger.LogInformation("Starting product rules validation...");
        var invalidPrices = await _new.Products.AnyAsync(p => p.Price < 0);
        AssertFalse(invalidPrices, "Negative prices");

        _logger.LogInformation("Price validation passed.");

        var invalidStatus = await _new.Products.AnyAsync(p =>
            p.Status != ProductStatus.Available &&
            p.Status != ProductStatus.Discontinued
        );

        AssertFalse(invalidStatus, "Invalid product status");

        _logger.LogInformation("Product status validation passed.\n");
    }

    private static void AssertEqual(int expected, int actual, string label)
    {
        if (expected != actual)
            throw new InvalidOperationException(
                $"{label} mismatch. Expected {expected}, got {actual}"
            );
    }

    private static void AssertFalse(bool condition, string message)
    {
        if (condition)
            throw new InvalidOperationException(message);
    }


}
