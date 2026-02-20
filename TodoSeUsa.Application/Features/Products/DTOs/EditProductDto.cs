using TodoSeUsa.Domain.Enums;

namespace TodoSeUsa.Application.Features.Products.DTOs;

public record EditProductDto
{
    public decimal Price { get; set; }

    public string Category { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public Body Body { get; set; }

    public string Size { get; set; } = string.Empty;

    public ProductStatus Status { get; set; }

    public ProductQuality Quality { get; set; }

    public string? Brand { get; set; }

    public string? Season { get; set; }

    public decimal? RefurbishmentCost { get; set; }

    public int ConsignmentId { get; set; }

    public int? SaleId { get; set; }

    public int? BoxId { get; set; }
}