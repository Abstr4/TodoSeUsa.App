using Microsoft.EntityFrameworkCore;
using static DataMigration.ETL.DTOs.ExtractionDTOs;

namespace DataMigration.Data;

public sealed class OldDbContext : DbContext
{
    public OldDbContext(DbContextOptions<OldDbContext> options) : base(options) { }

    public DbSet<OldClientRow> Client => Set<OldClientRow>();
    public DbSet<OldBillRow> Bill => Set<OldBillRow>();
    public DbSet<OldProductRow> Product => Set<OldProductRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OldClientRow>().HasNoKey();
        modelBuilder.Entity<OldBillRow>().HasNoKey();
        modelBuilder.Entity<OldProductRow>().HasNoKey();
    }
}