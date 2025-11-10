using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoSeUsa.Domain.Enums;

namespace TodoSeUsa.Infrastructure.Data.Configurations;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.UseTpcMappingStrategy();

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Method)
        .HasConversion(
            v => v.ToString(),
            v => Enum.Parse<PaymentMethod>(v)
        );

        builder.Property(s => s.Status)
    .HasConversion(
        v => v.ToString(),
        v => Enum.Parse<PaymentStatus>(v)
    );

        builder.Property(s => s.Notes).HasMaxLength(250);

        builder.ToTable("Sales")
            .HasQueryFilter(s => !s.DeletedAt.HasValue);
    }
}