using System.Reflection;
using TodoSeUsa.Application.Common.Interfaces;

namespace TodoSeUsa.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options), IApplicationDbContext
{
    public DbSet<Box> Boxes => Set<Box>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Consignment> Consignments => Set<Consignment>();
    public DbSet<LoanNote> LoanNotes => Set<LoanNote>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Person> People => Set<Person>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Provider> Providers => Set<Provider>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();
    public DbSet<Person> Persons => Set<Person>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.ClrType
                         .GetProperties()
                         .Where(p => p.PropertyType.IsEnum))
            {
                modelBuilder.Entity(entityType.Name)
                            .Property(property.Name)
                            .HasConversion<string>();
            }
        }

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        UpdateAuditableEntities();
        return await base.SaveChangesAsync(ct);
    }

    private void UpdateAuditableEntities()
    {
        foreach (var entry in ChangeTracker.Entries<BaseAuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.Now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.Now;
            }
        }
    }
}