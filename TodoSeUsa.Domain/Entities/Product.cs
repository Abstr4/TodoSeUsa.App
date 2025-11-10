namespace TodoSeUsa.Domain.Entities;

public class Product : BaseAuditableEntity
{
    public int Price { get; set; }

    public string Category { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public ProductQuality Quality { get; set; }

    public ProductStatus Status { get; private set; } = ProductStatus.Available;

    public int? RefurbishmentCost { get; set; }

    public Season? Season { get; set; }

    public int ConsignmentId { get; set; }

    public virtual Consignment Consignment { get; set; } = null!;

    public int? SaleId { get; set; }

    public virtual Sale? Sale { get; set; }

    public int? BoxId { get; set; }

    public Box? Box { get; set; }
}