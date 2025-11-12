namespace TodoSeUsa.Domain.Entities;

public class Payment : BaseAuditableEntity
{
    public decimal Amount { get; set; }

    public DateTime Date { get; set; } = DateTime.Now;

    public PaymentMethod Method { get; set; }

    public int SaleId { get; set; }

    public virtual Sale Sale { get; set; } = null!;
}