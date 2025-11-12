using TodoSeUsa.Domain.Enums;

namespace TodoSeUsa.Application.Features.Products.DTOs;

public record CreateProductDto
{
    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public string Category { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public Body Body { get; set; }

    public string Size { get; set; } = string.Empty;

    public ProductQuality Quality { get; set; }

    public Season? Season { get; set; }

    public decimal? RefurbishmentCost { get; set; }

    public int ConsignmentId { get; set; }

    public int? BoxId { get; set; }
}