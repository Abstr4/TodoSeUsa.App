using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TodoSeUsa.Infrastructure.Data.Configurations;

public class BoxConfiguration : IEntityTypeConfiguration<Box>
{

    public void Configure(EntityTypeBuilder<Box> builder)
    {
        builder.UseTpcMappingStrategy();

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Location).IsRequired().HasMaxLength(250);

        builder.ToTable("Boxes")
            .HasQueryFilter(b => !b.DeletedAt.HasValue);
    }
}