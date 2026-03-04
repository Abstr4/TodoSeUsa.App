using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TodoSeUsa.Infrastructure.Data.Configurations;

public class ConsignmentConfiguration : IEntityTypeConfiguration<Consignment>
{
    public void Configure(EntityTypeBuilder<Consignment> builder)
    {
        builder.UseTpcMappingStrategy();

        builder.ToTable("Consignments")
            .HasQueryFilter(c => !c.DeletedAt.HasValue);

        builder.Property(c => c.PublicId).IsRequired();

        builder.HasIndex(c => c.PublicId).IsUnique();

        builder.HasKey(c => c.Id);

        builder.HasMany(c => c.Products)
            .WithOne(c => c.Consignment)
            .HasForeignKey(c => c.ConsignmentId)
            .IsRequired()
        .OnDelete(DeleteBehavior.Restrict);

        builder.Property(c => c.Notes).HasMaxLength(250);
    }
}