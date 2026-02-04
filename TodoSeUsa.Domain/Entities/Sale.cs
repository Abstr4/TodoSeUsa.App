namespace TodoSeUsa.Domain.Entities;

public class Sale : BaseAuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal AmountPaid { get; set; }
    public SaleStatus Status { get; set; }

    public ICollection<SaleItem> Items { get; set; } = [];
    public ICollection<Payment> Payments { get; set; } = [];
    public DateTime DateIssued { get; set; } = DateTime.Now;
    public string? Notes { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelReason { get; set; }

    public int? ClientId { get; set; }
    public Client? Client { get; set; }
}