using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TodoSeUsa.Infrastructure.Data.Configurations;

public class ProviderConfiguration : IEntityTypeConfiguration<Provider>
{
    public void Configure(EntityTypeBuilder<Provider> builder)
    {
        builder.UseTpcMappingStrategy();

        builder.ToTable("Providers")
            .HasQueryFilter(b => !b.DeletedAt.HasValue);

        builder.HasKey(c => c.Id);

        builder.HasMany(p => p.Consignments)
            .WithOne(c => c.Provider)
            .HasForeignKey(c => c.ProviderId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Person)
           .WithOne(per => per.Provider)
           .HasForeignKey<Provider>(p => p.PersonId)
           .IsRequired()
           .OnDelete(DeleteBehavior.Restrict);

        builder.Property(p => p.CommissionPercent).HasPrecision(18, 4);
    }
}