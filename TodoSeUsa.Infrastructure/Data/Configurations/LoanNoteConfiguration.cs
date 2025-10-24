using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TodoSeUsa.Infrastructure.Data.Configurations;

public class LoanNoteConfiguration : IEntityTypeConfiguration<LoanNote>
{
    public void Configure(EntityTypeBuilder<LoanNote> builder)
    {
        builder.UseTpcMappingStrategy();

        builder.HasKey(c => c.Id);
        builder.ToTable("LoanNotes");

        builder.HasQueryFilter(b => b.DeletedAt == null);

        builder.HasMany(l => l.LoanedProducts)
               .WithOne(lp => lp.LoanNote)
               .HasForeignKey(lp => lp.LoanNoteId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}