using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TodoSeUsa.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.UseTpcMappingStrategy();

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Category).IsRequired().HasMaxLength(100);

        builder.Property(c => c.Description).HasMaxLength(250);

        builder.ToTable("Products")
            .HasQueryFilter(b => !b.DeletedAt.HasValue);
    }
}