using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoSeUsa.Domain.Enums;

namespace TodoSeUsa.Infrastructure.Data.Configurations;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.UseTpcMappingStrategy();

        builder.HasKey(s => s.Id);

        builder.ToTable("Sales")
            .HasQueryFilter(s => !s.DeletedAt.HasValue);

        builder.Property(s => s.Status)
        .HasConversion(
            v => v.ToString(),
            v => Enum.Parse<SaleStatus>(v)
        );

        builder.Property(s => s.Notes).HasMaxLength(250);

        builder.Property(p => p.TotalAmount)
        .IsRequired()
        .HasPrecision(18, 2);

        builder.Property(p => p.AmountPaid)
        .IsRequired()
        .HasPrecision(18, 2);
    }
}