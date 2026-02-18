using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TodoSeUsa.Infrastructure.Data.Configurations;

public class ConsignorConfiguration : IEntityTypeConfiguration<Consignor>
{
    public void Configure(EntityTypeBuilder<Consignor> builder)
    {
        builder.UseTpcMappingStrategy();

        builder.ToTable(
            "Consignors",
            t => t.HasCheckConstraint(
            "CK_Consignor_CommissionPercentPercent",
            "[CommissionPercent] BETWEEN 0 AND 100"));

        builder.Property(x => x.CommissionPercent)
            .HasPrecision(3, 0);

        builder.HasQueryFilter(b => b.DeletedAt == null);

        builder.HasKey(c => c.Id);

        builder.HasMany(p => p.Consignments)
            .WithOne(c => c.Consignor)
            .HasForeignKey(c => c.ConsignorId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Person)
           .WithOne(per => per.Consignor)
           .HasForeignKey<Consignor>(p => p.PersonId)
           .IsRequired()
           .OnDelete(DeleteBehavior.Restrict);
    }
}