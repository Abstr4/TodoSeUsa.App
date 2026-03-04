using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TodoSeUsa.Infrastructure.Data.Configurations;

public sealed class PayoutConfiguration : IEntityTypeConfiguration<Payout>
{
    public void Configure(EntityTypeBuilder<Payout> builder)
    {
        builder.HasQueryFilter(p => p.Consignor.DeletedAt == null);

        builder.HasKey(p => p.Id);

        builder.Property(p => p.TotalAmount)
            .HasPrecision(18, 2);

        builder.Property(p => p.PublicId).IsRequired();

        builder.HasIndex(p => p.PublicId).IsUnique();

        builder.HasOne(p => p.Consignor)
            .WithMany()
            .HasForeignKey(p => p.ConsignorId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasMany(p => p.Lines)
            .WithOne(p => p.Payout)
            .HasForeignKey(p => p.PayoutId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
