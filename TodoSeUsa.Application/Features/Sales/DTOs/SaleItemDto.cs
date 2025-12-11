using TodoSeUsa.Domain.Enums;

namespace TodoSeUsa.Application.Features.Sales.DTOs;

public class SaleItemDto
{
    public int ProductId { get; set; }

    public string ProductCode { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string Size { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public ProductQuality Quality { get; set; }

    public Body Body { get; set; }
}
