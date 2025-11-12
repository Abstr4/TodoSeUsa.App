using TodoSeUsa.Domain.Enums;

namespace TodoSeUsa.Application.Features.Payments.DTOs;

public record PaymentDto
{
    public int Id { get; init; }

    public decimal Amount { get; init; }

    public DateTime Date { get; init; }

    public PaymentMethod Method { get; init; }

    public int SaleId { get; init; }

    public DateTime CreatedAt { get; init; }
}
