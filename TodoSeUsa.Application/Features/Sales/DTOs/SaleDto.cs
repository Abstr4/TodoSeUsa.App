using TodoSeUsa.Domain.Enums;

namespace TodoSeUsa.Application.Features.Sales.DTOs;

public record SaleDto
{
    public int Id { get; init; }

    public PaymentStatus Status { get; init; }

    public PaymentMethod Method { get; init; }

    public int TotalProducts { get; init; }

    public int TotalPayments { get; init; }

    public DateTime DateIssued { get; init; }

    public string? ClientFirstName { get; init; }

    public string? ClientLastName { get; init; }

    public DateTime DueDate { get; init; }

    public string Notes { get; init; } = string.Empty;

    public int? ClientId { get; init; }

    public DateTime CreatedAt { get; init; }
}