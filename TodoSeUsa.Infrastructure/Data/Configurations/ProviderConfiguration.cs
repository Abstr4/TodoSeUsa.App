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

        builder.Property(p => p.CommissionPercent).HasPrecision(18, 4);

        builder.Property(c => c.PersonId).IsRequired();

        builder.HasIndex(c => c.PersonId).IsUnique();
    }
}