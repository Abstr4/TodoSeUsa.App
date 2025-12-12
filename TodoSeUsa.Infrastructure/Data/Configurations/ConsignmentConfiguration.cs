using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TodoSeUsa.Infrastructure.Data.Configurations;

public class ConsignmentConfiguration : IEntityTypeConfiguration<Consignment>
{
    public void Configure(EntityTypeBuilder<Consignment> builder)
    {
        builder.UseTpcMappingStrategy();

        builder.ToTable("Consignments")
            .HasQueryFilter(b => !b.DeletedAt.HasValue);

        builder.HasKey(c => c.Id);

        builder.HasMany(c => c.Products)
            .WithOne(p => p.Consignment)
            .HasForeignKey(p => p.ConsignmentId)
            .IsRequired()
        .OnDelete(DeleteBehavior.Restrict);

        builder.Property(c => c.Notes).HasMaxLength(250);
    }
}