using Microsoft.EntityFrameworkCore.Storage;

namespace TodoSeUsa.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Box> Boxes { get; }

    DbSet<Client> Clients { get; }

    DbSet<Product> Products { get; }

    DbSet<Payment> Payments { get; }

    DbSet<Consignment> Consignments { get; }

    DbSet<Provider> Providers { get; }

    DbSet<Sale> Sales { get; }

    DbSet<SaleItem> SaleItems { get; }

    DbSet<LoanNote> LoanNotes { get; }

    DbSet<Reservation> Reservations { get; }

    DbSet<Person> Persons { get; }

    DbSet<ProductImage> ProductImages { get; }

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default);

    Task<int> SaveChangesAsync(CancellationToken ct);
}