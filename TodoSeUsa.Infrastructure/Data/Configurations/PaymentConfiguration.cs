using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TodoSeUsa.Infrastructure.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.UseTpcMappingStrategy();

        builder.HasKey(c => c.Id);

        builder.ToTable("Payments")
            .HasQueryFilter(b => !b.DeletedAt.HasValue);
    }
}