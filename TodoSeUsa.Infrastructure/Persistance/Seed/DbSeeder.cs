namespace TodoSeUsa.Infrastructure.Persistance.Seed;

using Microsoft.EntityFrameworkCore;
using TodoSeUsa.Application.Features.Consignments.Services;
using TodoSeUsa.Domain.Entities;
using TodoSeUsa.Domain.Enums;

public sealed class DbSeeder
{
    private readonly UniqueConsignmentCodeService _codeService;

    public DbSeeder(UniqueConsignmentCodeService codeService)
    {
        _codeService = codeService;
    }

    public async Task SeedAsync(ApplicationDbContext context)
    {
        await SeedBoxesAsync(context);
        await SeedPeopleAsync(context);
        await SeedConsignorsAsync(context);
        await SeedConsignmentsAsync(context);
        await SeedProductsAsync(context);
    }

    private static async Task SeedBoxesAsync(ApplicationDbContext context)
    {
        if (await context.Boxes.AnyAsync())
            return;

        var boxes = new List<Box>
        {
            new() { Location = "Main Warehouse" },
            new() { Location = "Secondary Storage" }
        };

        await context.Boxes.AddRangeAsync(boxes);
        await context.SaveChangesAsync();
    }

    private static async Task SeedPeopleAsync(ApplicationDbContext context)
    {
        if (await context.Persons.AnyAsync())
            return;

        var people = new List<Person>
        {
            new() { FirstName = "John", LastName = "Doe", EmailAddress = "john@example.com", PhoneNumber = "123456789", Address = "Av. Colón 949"},
            new() { FirstName = "Jane", LastName = "Smith", EmailAddress = "jane@example.com", PhoneNumber = "987654321" },
            new() { FirstName = "Carlos", LastName = "Pérez", EmailAddress = "carlos@example.com", PhoneNumber = "555111222", Address = "Av. Los Álamos 259" },
        };

        await context.Persons.AddRangeAsync(people);
        await context.SaveChangesAsync();
    }

    private static async Task SeedConsignorsAsync(ApplicationDbContext context)
    {
        if (await context.Consignors.AnyAsync())
            return;

        var person = await context.Persons.FirstAsync();
        var consignor = new Consignor
        {
            PersonId = person.Id,
            CommissionPercent = 10m
        };

        await context.Consignors.AddAsync(consignor);
        await context.SaveChangesAsync();
    }

    public async Task SeedConsignmentsAsync(ApplicationDbContext context, CancellationToken ct = default)
    {
        if (await context.Consignments.AnyAsync(ct))
            return;

        var consignor = await context.Consignors.FirstAsync(ct);

        var consignments = new List<Consignment>
        {
            new()
            {
                ConsignorId = consignor.Id,
                Code = await _codeService.GenerateAsync(ct),
                Notes = "First consignment"
            },
            new()
            {
                ConsignorId = consignor.Id,
                Code = await _codeService.GenerateAsync(ct),
                Notes = "Second consignment"
            }
        };

        await context.Consignments.AddRangeAsync(consignments, ct);
        await context.SaveChangesAsync(ct);
    }

    private static async Task SeedProductsAsync(ApplicationDbContext context)
    {
        if (await context.Products.AnyAsync())
            return;

        var consignment = await context.Consignments.FirstAsync();
        var box = await context.Boxes.FirstAsync();

        var products = new List<Product>
        {
            new() { Price = 100.00m, Category = "Pants", Description = "Denim jeans", Quality = ProductQuality.New, ConsignmentId = consignment.Id, BoxId = box.Id, Size = "S" },

            new() { Price = 250.50m, Category = "Tops", Description = "Cotton t-shirt", Quality = ProductQuality.Good, ConsignmentId = consignment.Id, BoxId = box.Id, Body = Body.Male, Size = "M" },

            new() { Price = 800.00m, Category = "Outerwear", Description = "Winter jacket", Quality = ProductQuality.Fair, ConsignmentId = consignment.Id, BoxId = box.Id, Body = Body.Female, Size = "L" },

            new() { Price = 2000.00m, Category = "Dresses", Description = "Evening dress", Quality = ProductQuality.New, ConsignmentId = consignment.Id, BoxId = box.Id, Body = Body.Female, Size = "S" },

            new() { Price = 4030.75m, Category = "Coats", Description = "Long wool coat", Quality = ProductQuality.Poor, ConsignmentId = consignment.Id, BoxId = box.Id, Body = Body.Female, Size = "XXXL" },

            new() { Price = 6050.75m, Category = "Sweaters", Description = "Knit sweater", Quality = ProductQuality.LikeNew, ConsignmentId = consignment.Id, BoxId = box.Id, Size = "L" },

            new() { Price = 12980.75m, Category = "Shirts", Description = "Formal button-up shirt", Quality = ProductQuality.Fair, ConsignmentId = consignment.Id, BoxId = box.Id, Body = Body.Male, Size = "XS" },

            new() { Price = 8130.75m, Category = "Hoodies", Description = "Unisex hoodie", Quality = ProductQuality.Fair, ConsignmentId = consignment.Id, BoxId = box.Id, Body = Body.Unisex, Size = "XXL" }
        };

        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();
    }
}