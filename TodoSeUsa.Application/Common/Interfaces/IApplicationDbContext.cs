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

    DbSet<LoanNote> LoanNotes { get; }

    DbSet<Reservation> Reservations { get; }

    DbSet<Person> Persons { get; }

    Task<int> SaveChangesAsync(CancellationToken ct);
}