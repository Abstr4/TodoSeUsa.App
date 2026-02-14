namespace TodoSeUsa.Domain.Entities;

public class Product : BaseAuditableEntity
{
    public decimal Price { get; set; }
    public string Size { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ProductQuality Quality { get; set; }
    public ProductStatus Status { get; set; }
    public Body Body { get; set; }
    public string? Brand { get; set; } = string.Empty;
    public string? Season { get; set; }
    public decimal? RefurbishmentCost { get; set; }

    public ICollection<ProductImage> Images { get; set; } = [];

    public DateTime? PayoutDate { get; set; }

    public int ConsignmentId { get; set; }
    public Consignment Consignment { get; set; } = null!;

    public int? SaleId { get; set; }
    public Sale? Sale { get; set; }

    public int? BoxId { get; set; }
    public Box? Box { get; set; }
}