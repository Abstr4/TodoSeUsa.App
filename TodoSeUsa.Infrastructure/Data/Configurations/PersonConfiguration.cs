using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TodoSeUsa.Infrastructure.Data.Configurations;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.UseTpcMappingStrategy();

        builder.ToTable("People", t =>
        {
            t.HasCheckConstraint(
                "CK_Person_EmailOrPhone",
                "[EmailAddress] IS NOT NULL OR [PhoneNumber] IS NOT NULL");
        }).HasQueryFilter(b => !b.DeletedAt.HasValue);

        builder.HasKey(p => p.Id);

        builder.Property(p => p.FullName)
            .HasComputedColumnSql("[FirstName] + ' ' + [LastName]", stored: false);

        builder.Property(p => p.ContactInfo)
            .HasComputedColumnSql(
                "CONCAT_WS(' | ', EmailAddress, PhoneNumber, Address)",
                stored: false
            );

        builder.Property(p => p.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(p => p.LastName).IsRequired().HasMaxLength(100);

        builder.Property(p => p.EmailAddress).HasMaxLength(100);
        builder.Property(p => p.PhoneNumber).HasMaxLength(20);

        builder.HasOne(p => p.Client)
            .WithOne(c => c.Person)
            .HasForeignKey<Client>(c => c.PersonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Consignor)
            .WithOne(pr => pr.Person)
            .HasForeignKey<Consignor>(pr => pr.PersonId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}