using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TodoSeUsa.Infrastructure.Data.Configurations;

public class LoanedProductConfiguration : IEntityTypeConfiguration<LoanedProduct>
{
    public void Configure(EntityTypeBuilder<LoanedProduct> builder)
    {
        builder.UseTpcMappingStrategy();

        builder.HasKey(lp => lp.Id);
        builder.ToTable("LoanedProducts");

        builder.HasQueryFilter(lp => lp.DeletedAt == null && lp.LoanNote.DeletedAt == null);
    }
}