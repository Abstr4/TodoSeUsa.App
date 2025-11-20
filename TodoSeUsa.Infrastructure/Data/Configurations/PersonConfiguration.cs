namespace TodoSeUsa.Infrastructure.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoSeUsa.Domain.Entities;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.UseTpcMappingStrategy();

        builder.HasKey(p => p.Id);

        builder.Property(p => p.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(p => p.LastName).IsRequired().HasMaxLength(100);

        builder.Property(p => p.EmailAddress).HasMaxLength(100);
        builder.Property(p => p.PhoneNumber).HasMaxLength(20);

        builder.HasIndex(p => p.EmailAddress)
            .IsUnique()
            .HasFilter("[EmailAddress] IS NOT NULL");

        builder.HasIndex(p => p.PhoneNumber)
            .IsUnique()
            .HasFilter("[PhoneNumber] IS NOT NULL");

        builder.HasOne(p => p.Client)
            .WithOne(c => c.Person)
            .HasForeignKey<Client>(c => c.PersonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Provider)
            .WithOne(pr => pr.Person)
            .HasForeignKey<Provider>(pr => pr.PersonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("People", t =>
        {
            t.HasCheckConstraint(
                "CK_Person_EmailOrPhone",
                "[EmailAddress] IS NOT NULL OR [PhoneNumber] IS NOT NULL");
        })
        .HasQueryFilter(b => !b.DeletedAt.HasValue);
    }
}