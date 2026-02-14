namespace TodoSeUsa.Domain.Entities;

public class SaleItem
{
    public int Id { get; set; }

    public int SaleId { get; set; }
    public Sale Sale { get; set; } = null!;

    // snapshot
    public int? ProductId { get; set; }
    public Product? Product { get; set; }

    public decimal Price { get; set; }

    public string Size { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ProductQuality Quality { get; set; }
    public Body Body { get; set; }
    public string? Brand { get; set; }
    public string? Season { get; set; }

    public int ConsignorId { get; set; }
    public decimal ConsignorPercent { get; set; }
    public decimal AmountPaidOut { get; set; }

    public DateTime CreatedAt { get; set; }

    // lifecycle
    public DateTime? ReturnedAt { get; set; }
    public string? ReturnReason { get; set; }
}