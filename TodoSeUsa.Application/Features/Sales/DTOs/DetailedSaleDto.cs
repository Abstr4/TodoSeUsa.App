using TodoSeUsa.Domain.Enums;

namespace TodoSeUsa.Application.Features.Sales.DTOs;

public record DetailedSaleDto
{
    public int Id { get; init; }

    public decimal TotalAmount { get; init; }

    public decimal AmountPaid { get; init; }

    public SaleStatus Status { get; init; }

    public int TotalItems { get; init; }

    public int TotalPayments { get; init; }

    public DateTime DateIssued { get; init; }

    public string? Notes { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime? UpdatedAt { get; init; }
}
