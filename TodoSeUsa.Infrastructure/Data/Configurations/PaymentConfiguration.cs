using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoSeUsa.Domain.Enums;

namespace TodoSeUsa.Infrastructure.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.UseTpcMappingStrategy();

        builder.ToTable("Payments")
            .HasQueryFilter(b => !b.DeletedAt.HasValue);

        builder.HasKey(c => c.Id);

        builder.Property(p => p.Amount)
        .HasPrecision(18, 2);

        builder.Property(s => s.Method)
        .HasConversion(
            v => v.ToString(),
            v => Enum.Parse<PaymentMethod>(v)
        );
    }
}