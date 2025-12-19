using TodoSeUsa.Domain.Enums;

namespace TodoSeUsa.Application.Features.Products.DTOs;

public record ProductSimpleDto
{
    public int Id { get; init; }

    public string Code { get; init; } = string.Empty;

    public decimal Price { get; init; }

    public string Category { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public ProductStatus Status { get; init; }

    public Body Body { get; init; }

    public string Size { get; init; } = string.Empty;

    public ProductQuality Quality { get; init; }

    public string? Season { get; init; }

    public decimal? RefurbishmentCost { get; init; }

    public DateTime CreatedAt { get; init; }
}