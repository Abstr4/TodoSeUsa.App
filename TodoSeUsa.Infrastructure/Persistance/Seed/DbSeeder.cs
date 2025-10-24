namespace TodoSeUsa.Infrastructure.Persistance.Seed;

using Microsoft.EntityFrameworkCore;
using TodoSeUsa.Domain.Entities;
using TodoSeUsa.Domain.Enums;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        await SeedBoxesAsync(context);
        await SeedPeopleAsync(context);
        await SeedProvidersAsync(context);
        await SeedConsignmentsAsync(context);
        await SeedProductsAsync(context);
        await SeedClientsAsync(context);
        await SeedSalesAsync(context);
        await SeedPaymentsAsync(context);
        await SeedReservationsAsync(context);
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

    private static async Task SeedConsignmentsAsync(ApplicationDbContext context)
    {
        if (await context.Consignments.AnyAsync())
            return;

        var provider = await context.Providers.FirstAsync();

        var consignments = new List<Consignment>
        {
            new() { ProviderId = provider.Id, Notes = "First consignment" },
            new() { ProviderId = provider.Id, Notes = "Second consignment" }
        };

        await context.Consignments.AddRangeAsync(consignments);
        await context.SaveChangesAsync();
    }

    private static async Task SeedProductsAsync(ApplicationDbContext context)
    {
        if (await context.Products.AnyAsync())
            return;

        var consignment = await context.Consignments.FirstAsync();
        var box = await context.Boxes.FirstAsync();

        var products = new List<Product>
        {
            new() { Price = 100, Category = "Electronics", Description = "Wireless Mouse", Quality = Quality.New, ConsignmentId = consignment.Id, BoxId = box.Id },
            new() { Price = 250, Category = "Home", Description = "Blender", Quality = Quality.Good, ConsignmentId = consignment.Id, BoxId = box.Id },
            new() { Price = 80, Category = "Clothing", Description = "Jacket", Quality = Quality.Fair, ConsignmentId = consignment.Id, BoxId = box.Id }
        };

        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();
    }

    private static async Task SeedClientsAsync(ApplicationDbContext context)
    {
        if (await context.Clients.AnyAsync())
            return;

        var person = await context.People.Skip(1).FirstAsync(); // different person than provider

        var client = new Client
        {
            PersonId = person.Id
        };

        await context.Clients.AddAsync(client);
        await context.SaveChangesAsync();
    }

    private static async Task SeedSalesAsync(ApplicationDbContext context)
    {
        if (await context.Sales.AnyAsync())
            return;

        var client = await context.Clients.FirstAsync();
        var products = await context.Products.Take(2).ToListAsync();

        var sale = new Sale
        {
            Status = PaymentStatus.Paid,
            Method = PaymentMethod.Cash,
            ClientId = client.Id,
            Notes = "Test sale",
            DateIssued = DateTime.Now,
            DueDate = DateTime.Now.AddDays(30),
            Products = products
        };

        await context.Sales.AddAsync(sale);
        await context.SaveChangesAsync();
    }

    private static async Task SeedPaymentsAsync(ApplicationDbContext context)
    {
        if (await context.Payments.AnyAsync())
            return;

        var sale = await context.Sales.FirstAsync();

        var payment = new Payment
        {
            Amount = 350,
            Date = DateTime.Now,
            Method = PaymentMethod.Cash,
            SaleId = sale.Id
        };

        await context.Payments.AddAsync(payment);
        await context.SaveChangesAsync();
    }

    private static async Task SeedReservationsAsync(ApplicationDbContext context)
    {
        if (await context.Reservations.AnyAsync())
            return;

        var client = await context.Clients.FirstAsync();
        var product = await context.Products.Skip(1).FirstAsync();

        var reservation = new Reservation
        {
            ClientId = client.Id,
            DateIssued = DateTime.Now,
            ExpiresAt = DateTime.Now.AddDays(7),
            Status = ReservationStatus.Active,
            Products = new List<Product> { product }
        };

        await context.Reservations.AddAsync(reservation);
        await context.SaveChangesAsync();
    }
}
