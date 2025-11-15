using TodoSeUsa.Domain.Enums;

namespace TodoSeUsa.Application.Features.Products.DTOs;

public record ProductDto
{
    public int Id { get; init; }

    public decimal Price { get; init; }

    public int Quantity { get; init; }

    public string Category { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public ProductStatus Status { get; init; }

    public Body Body { get; init; }

    public string Size { get; init; } = string.Empty;

    public ProductQuality Quality { get; init; }

    public Season? Season { get; init; }

    public decimal? RefurbishmentCost { get; init; }

    public int ConsignmentId { get; init; }

    public int? SaleId { get; init; }

    public int? BoxId { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime UpdatedAt { get; init; }
}