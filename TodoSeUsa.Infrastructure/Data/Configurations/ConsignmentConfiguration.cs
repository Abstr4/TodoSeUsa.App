using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TodoSeUsa.Infrastructure.Data.Configurations;

public class ConsignmentConfiguration : IEntityTypeConfiguration<Consignment>
{
    public void Configure(EntityTypeBuilder<Consignment> builder)
    {
        builder.UseTpcMappingStrategy();

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Notes).HasMaxLength(250);

        builder.ToTable("Consignments")
            .HasQueryFilter(b => !b.DeletedAt.HasValue);
    }
}