using TodoSeUsa.Domain.Enums;

namespace TodoSeUsa.Application.Features.Sales.DTOs;

public record SaleDto
{
    public int Id { get; init; }

    public string Code { get; init; } = string.Empty;

    public decimal TotalAmount { get; init; }

    public decimal AmountPaid { get; init; }

    public SaleStatus Status { get; init; }

    public string? Notes { get; init; }

    public DateTime DateIssued { get; init; }

    public DateTime CreatedAt { get; init; }

}