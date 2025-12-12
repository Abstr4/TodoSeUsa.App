using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoSeUsa.Domain.Enums;

namespace TodoSeUsa.Infrastructure.Data.Configurations;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.UseTpcMappingStrategy();

        builder.ToTable("Sales");

        builder.HasKey(s => s.Id);

        builder.HasMany(s => s.Items)
            .WithOne(si => si.Sale)
            .HasForeignKey(si => si.SaleId)
            .IsRequired()
        .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.Payments)
            .WithOne(si => si.Sale)
            .HasForeignKey(si => si.SaleId)
            .IsRequired()
        .OnDelete(DeleteBehavior.Restrict);

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