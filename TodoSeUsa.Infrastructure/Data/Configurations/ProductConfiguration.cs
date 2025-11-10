using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoSeUsa.Domain.Enums;

namespace TodoSeUsa.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.UseTpcMappingStrategy();

        builder.HasKey(c => c.Id);

        builder.Property(x => x.Status)
        .HasConversion(
            v => v.ToString(),
            v => Enum.Parse<ProductStatus>(v)
        );

        builder.Property(x => x.Quality)
        .HasConversion(
            v => v.ToString(),
            v => Enum.Parse<ProductQuality>(v)
        );

        builder.Property(v => v.Season)
            .HasConversion(
                v => v.HasValue ? v.Value.ToString() : null,
                v => v != null ? Enum.Parse<Season>(v) : null
            );

        builder.Property(c => c.Category).IsRequired().HasMaxLength(100);

        builder.Property(c => c.Description).HasMaxLength(250);

        builder.ToTable("Products")
            .HasQueryFilter(b => !b.DeletedAt.HasValue);
    }
}