using TodoSeUsa.Application.Features.Payments.DTOs;

namespace TodoSeUsa.Application.Features.Sales.DTOs;

public record CreateSaleDto
{
    public List<int> ProductsIds { get; set; } = [];

    public List<CreatePaymentDto> Payments { get; set; } = [];

    public DateTime? DateIssued { get; set; }

    public string? Notes { get; set; }
}