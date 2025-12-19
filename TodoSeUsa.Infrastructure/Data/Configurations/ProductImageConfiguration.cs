using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoSeUsa.Infrastructure.Data.Configurations;
public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.UseTpcMappingStrategy();

        builder.ToTable("ProductImages")
            .HasQueryFilter(b => !b.DeletedAt.HasValue);

        builder.HasKey(i => i.Id);

        builder.HasIndex(i => new { i.ProductId, i.IsMain })
            .IsUnique()
            .HasFilter("[IsMain] = 1");
    }
}