using TodoSeUsa.Domain.Enums;

namespace TodoSeUsa.Application.Features.Sales.DTOs;

public class PaymentDto
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    public PaymentMethod Method { get; set; }

    public int SaleId { get; set; }

    public DateTime? RefundedAt { get; set; }
    public string? RefundReason { get; set; }
}
