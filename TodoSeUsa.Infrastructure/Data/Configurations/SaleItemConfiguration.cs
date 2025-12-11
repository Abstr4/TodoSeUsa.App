using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoSeUsa.Domain.Enums;

namespace TodoSeUsa.Infrastructure.Data.Configurations;

public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("SaleItems");

        builder.HasKey(c => c.Id);

        builder.HasOne(si => si.Sale)
            .WithMany(s => s.Items)
            .HasForeignKey(si => si.SaleId);

        builder.Property(p => p.Price)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(x => x.Size)
            .HasMaxLength(50);

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

        builder.Property(c => c.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Description)
            .HasMaxLength(250);
    }
}