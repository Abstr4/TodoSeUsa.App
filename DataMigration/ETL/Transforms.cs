using TodoSeUsa.Domain.Entities;
using TodoSeUsa.Domain.Enums;
using static DataMigration.ETL.DTOs.ExtractionDTOs;

namespace DataMigration.ETL;

public static class Transforms
{
    public static Person ToPerson(OldClientRow c) => new()
    {
        FirstName = c.FirstName,
        LastName = c.LastName,
        EmailAddress = c.Email,
        PhoneNumber = c.PhoneNumber,
        Address = c.Address
    };

    public static Provider ToProvider(Person p) => new()
    {
        PersonId = p.Id,
        CommissionPercent = 0m
    };

    public static Consignment ToConsignment(OldBillRow bill, int providerId) => new()
    {
        Code = $"LEGACY-{bill.BillId}",
        DateIssued = bill.DateCreated,
        ProviderId = providerId
    };

    public static Product ToProduct(OldProductRow p, int consignmentId) => new()
    {
        Id = p.ProductId,
        Price = Convert.ToDecimal(p.Price),
        Category = p.Type,
        Description = p.Description ?? string.Empty,
        Quality = MapQuality(p.Condition),
        Status = MapStatus(p.State),
        ConsignmentId = consignmentId
    };

    private static ProductQuality MapQuality(string? value)
    {
        var v = Normalize(value);

        if (StartsWithAny(v, "nuevo", "nueva", "nuev"))
            return ProductQuality.New;

        if (ContainsAny(v, "como nuevo", "casi nuevo", "excelente"))
            return ProductQuality.LikeNew;

        if (ContainsAny(v, "bueno", "buen estado", "ok"))
            return ProductQuality.Good;

        if (StartsWithAny(v, "usado", "usada") || v.Contains("regular"))
            return ProductQuality.Fair;

        if (ContainsAny(v, "malo", "roto", "dañado", "pobre"))
            return ProductQuality.Poor;

        return ProductQuality.Good;
    }

    private static ProductStatus MapStatus(string? state)
    {
        var v = Normalize(state);

        if (ContainsAny(v, "baja", "desc", "discon"))
            return ProductStatus.Discontinued;

        return ProductStatus.Available;
    }

    private static string Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : value.Trim().ToLowerInvariant();

    private static bool ContainsAny(string value, params string[] terms) =>
        terms.Any(value.Contains);

    private static bool StartsWithAny(string value, params string[] terms) =>
        terms.Any(value.StartsWith);
}