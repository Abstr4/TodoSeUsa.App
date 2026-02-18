using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TodoSeUsa.Infrastructure.Data.Configurations;

public sealed class PayoutLineConfiguration : IEntityTypeConfiguration<PayoutLine>
{
    public void Configure(EntityTypeBuilder<PayoutLine> builder)
    {
        builder.HasQueryFilter(x => x.Payout.Consignor.DeletedAt == null);

        builder.Property(x => x.AmountPaid)
            .HasPrecision(18, 2);

        builder.HasOne(x => x.Payout)
            .WithMany(x => x.Lines)
            .HasForeignKey(x => x.PayoutId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.SaleItem)
            .WithMany()
            .HasForeignKey(x => x.SaleItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

