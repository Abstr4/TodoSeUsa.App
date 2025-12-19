using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TodoSeUsa.Infrastructure.Data.Configurations;

public class BoxConfiguration : IEntityTypeConfiguration<Box>
{
    public void Configure(EntityTypeBuilder<Box> builder)
    {
        builder.UseTpcMappingStrategy();

        builder.ToTable("Boxes")
            .HasQueryFilter(b => !b.DeletedAt.HasValue);

        builder.HasKey(c => c.Id);

        builder.HasMany(b => b.Products)
            .WithOne(p => p.Box)
            .HasForeignKey(p => p.BoxId)
        .OnDelete(DeleteBehavior.Restrict);

        builder.Property(p => p.Code)
            .HasComputedColumnSql("'BOX-' + RIGHT('000' + CAST(Id AS VARCHAR(3)), 3)", stored: true);

        builder.Property(c => c.Location).IsRequired().HasMaxLength(250);
    }
}