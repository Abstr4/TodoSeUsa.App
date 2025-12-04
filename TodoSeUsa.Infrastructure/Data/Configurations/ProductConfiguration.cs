using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoSeUsa.Domain.Enums;

namespace TodoSeUsa.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.UseTpcMappingStrategy();

        builder.ToTable("Products")
            .HasQueryFilter(b => !b.DeletedAt.HasValue);

        builder.HasKey(c => c.Id);

        builder.Property(p => p.RefurbishmentCost)
        .HasPrecision(18, 2);

        builder.Property(p => p.Price)
         .IsRequired()
        .HasPrecision(18, 2);

        builder.Property(x => x.Size)
            .HasMaxLength(50);

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

        builder.Property(x => x.Body)
        .HasConversion(
            v => v.ToString(),
            v => Enum.Parse<Body>(v)
        );

        builder.Property(c => c.Category).IsRequired().HasMaxLength(100);

        builder.Property(c => c.Description).HasMaxLength(250);

        builder.Property(c => c.Season).HasMaxLength(250);
    }
}