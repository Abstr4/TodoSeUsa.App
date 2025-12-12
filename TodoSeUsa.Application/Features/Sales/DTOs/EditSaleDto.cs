namespace TodoSeUsa.Application.Features.Sales.DTOs;

public record EditSaleDto
{
    public int Id { get; init; }

    public string? Notes { get; set; }
}
