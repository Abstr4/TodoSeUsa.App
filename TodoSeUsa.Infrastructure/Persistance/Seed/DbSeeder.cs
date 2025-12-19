namespace TodoSeUsa.Infrastructure.Persistance.Seed;

using Microsoft.EntityFrameworkCore;
using TodoSeUsa.Application.Common.Services;
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
        await SeedProvidersAsync(context);
        await SeedConsignmentsAsync(context);
        await SeedProductsAsync(context);
        // await SeedSalesAsync(context);
        // await SeedClientsAsync(context);
        // await SeedReservationsAsync(context);
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
        if (await context.People.AnyAsync())
            return;

        var people = new List<Person>
        {
            new() { FirstName = "John", LastName = "Doe", EmailAddress = "john@example.com", PhoneNumber = "123456789", Address = "Av. Colón 949"},
            new() { FirstName = "Jane", LastName = "Smith", EmailAddress = "jane@example.com", PhoneNumber = "987654321" },
            new() { FirstName = "Carlos", LastName = "Pérez", EmailAddress = "carlos@example.com", PhoneNumber = "555111222", Address = "Av. Los Álamos 259" },
        };

        await context.People.AddRangeAsync(people);
        await context.SaveChangesAsync();
    }

    private static async Task SeedProvidersAsync(ApplicationDbContext context)
    {
        if (await context.Providers.AnyAsync())
            return;

        var person = await context.People.FirstAsync();
        var provider = new Provider
        {
            PersonId = person.Id,
            CommissionPercent = 10m
        };

        await context.Providers.AddAsync(provider);
        await context.SaveChangesAsync();
    }

    public async Task SeedConsignmentsAsync(ApplicationDbContext context, CancellationToken ct = default)
    {
        if (await context.Consignments.AnyAsync(ct))
            return;

        var provider = await context.Providers.FirstAsync(ct);

        var consignments = new List<Consignment>
        {
            new()
            {
                ProviderId = provider.Id,
                Code = await _codeService.GenerateAsync(ct),
                Notes = "First consignment"
            },
            new()
            {
                ProviderId = provider.Id,
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
            new() { Price = 100.00m, Category = "Electronics", Description = "Wireless Mouse", Quality = ProductQuality.New, ConsignmentId = consignment.Id, BoxId = box.Id, Size = "S"},
            new() { Price = 250.50m, Category = "Home", Description = "Blender", Quality = ProductQuality.Good, ConsignmentId = consignment.Id, BoxId = box.Id, Body = Body.Male, Size = "XS" },
            new() { Price = 800.00m, Category = "Clothing", Description = "Jacket", Quality = ProductQuality.Fair, ConsignmentId = consignment.Id, BoxId = box.Id, Body = Body.Female, Size = "XXXL" },
            new() { Price = 2000.00m, Category = "CategoryTest1", Description = "DescriptionTest1", Quality = ProductQuality.New, ConsignmentId = consignment.Id, BoxId = box.Id, Body = Body.Female, Size = "XXXXXXL" },
            new() { Price = 4030.75m, Category = "CategoryTest2", Description = "DescriptionTest2", Quality = ProductQuality.Poor, ConsignmentId = consignment.Id, BoxId = box.Id, Body = Body.Female, Size = "XXXXXXXXXXXXXXL" },
            new() { Price = 6050.75m, Category = "CategoryTest3", Description = "DescriptionTest3", Quality = ProductQuality.LikeNew, ConsignmentId = consignment.Id, BoxId = box.Id, Size = "L" },
            new() { Price = 12980.75m, Category = "CategoryTest4", Description = "DescriptionTest4", Quality = ProductQuality.Fair, ConsignmentId = consignment.Id, BoxId = box.Id, Body = Body.Male, Size = "XL" },
            new() { Price = 8130.75m, Category = "CategoryTest5", Description = "DescriptionTest5", Quality = ProductQuality.Fair, ConsignmentId = consignment.Id, BoxId = box.Id, Body = Body.Unisex, Size = "XXL" }
        };

        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();
    }

    private static async Task SeedSalesAsync(ApplicationDbContext context)
    {
        if (await context.Sales.AnyAsync())
            return;

        var sales = new List<Sale>
    {
        new()
        {
            DateIssued = DateTime.Now,
            Notes = "Sample sale 1",
            Items =
            {
                new()
                {
                    ProductId = 10,
                    Price = 100,
                    Size = "M",
                    Category = "Shirt",
                    Description = "Blue shirt",
                    Quality = ProductQuality.Good,
                    Body = Body.Male,
                    CreatedAt = DateTime.Now
                },
                new()
                {
                    ProductId = 11,
                    Price = 200,
                    Size = "L",
                    Category = "Pants",
                    Description = "Black pants",
                    Quality = ProductQuality.Fair,
                    Body = Body.Female,
                    CreatedAt = DateTime.Now
                }
            },
            Payments =
            {
                new()
                {
                    Amount = 150,
                    Method = PaymentMethod.Cash,
                    Date = DateTime.Now
                }
            }
        },

        new()
        {
            DateIssued = DateTime.Now,
            Notes = "Sample sale 2",
            Items =
            {
                new()
                {
                    ProductId = 12,
                    Price = 500,
                    Size = "S",
                    Category = "Dress",
                    Description = "Red dress",
                    Quality = ProductQuality.Good,
                    Body = Body.Unisex,
                    CreatedAt = DateTime.Now
                }
            },
            Payments =
            {
                new()
                {
                    Amount = 500,
                    Method = PaymentMethod.CreditCard,
                    Date = DateTime.Now
                }
            }
        }
    };

        foreach (var sale in sales)
        {
            sale.TotalAmount = sale.Items.Sum(i => i.Price);
            sale.AmountPaid = sale.Payments.Sum(p => p.Amount);
            if (sale.AmountPaid == 0)
                sale.Status = SaleStatus.Pending;
            else if (sale.AmountPaid < sale.TotalAmount)
                sale.Status = SaleStatus.PartiallyPaid;
            else
                sale.Status = SaleStatus.Paid;

        }

        await context.Sales.AddRangeAsync(sales);
        await context.SaveChangesAsync();
    }

}