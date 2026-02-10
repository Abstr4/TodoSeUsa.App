using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TodoSeUsa.Infrastructure.Data.Configurations;

public class ConsignorConfiguration : IEntityTypeConfiguration<Consignor>
{
    public void Configure(EntityTypeBuilder<Consignor> builder)
    {
        builder.UseTpcMappingStrategy();

        builder.ToTable("Consignors")
            .HasQueryFilter(b => !b.DeletedAt.HasValue);

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

        builder.Property(p => p.CommissionPercent).HasPrecision(18, 4);
    }
}