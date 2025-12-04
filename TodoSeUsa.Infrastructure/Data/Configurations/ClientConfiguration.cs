using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TodoSeUsa.Infrastructure.Data.Configurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.UseTpcMappingStrategy();

        builder.ToTable("Clients")
            .HasQueryFilter(b => !b.DeletedAt.HasValue);

        builder.HasKey(c => c.Id);

        builder.Property(c => c.PersonId).IsRequired();

        builder.HasIndex(c => c.PersonId).IsUnique();
    }
}