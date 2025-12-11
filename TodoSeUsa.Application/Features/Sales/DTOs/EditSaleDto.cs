namespace TodoSeUsa.Application.Features.Sales.DTOs;

public record EditSaleDto
{
    public DateTime DateIssued { get; set; }

    public string? Notes { get; set; }
}
