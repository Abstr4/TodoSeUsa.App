using TodoSeUsa.Domain.Enums;

namespace TodoSeUsa.Application.Features.Payments.DTOs;

public record CreatePaymentDto
{
    public decimal Amount { get; set; }

    public PaymentMethod Method { get; set; }

    public DateTime? Date { get; set; }

    public string? Notes { get; set; }
}