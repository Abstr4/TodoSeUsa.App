using TodoSeUsa.Application.Features.Payments.DTOs;

namespace TodoSeUsa.Application.Features.Sales.DTOs;

public record CreateSaleDto
{
    public List<string> ProductCodes { get; set; } = [];

    public List<CreatePaymentDto> Payments { get; set; } = [];

    public DateTime? DateIssued { get; set; }

    public string? Notes { get; set; }
}