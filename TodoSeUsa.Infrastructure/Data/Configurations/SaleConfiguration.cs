using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TodoSeUsa.Infrastructure.Data.Configurations;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.UseTpcMappingStrategy();

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Notes).HasMaxLength(250);

        builder.ToTable("Sales")
            .HasQueryFilter(b => !b.DeletedAt.HasValue);
    }
}