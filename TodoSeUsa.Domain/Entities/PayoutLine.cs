namespace TodoSeUsa.Domain.Entities;

public class PayoutLine
{
    public int Id { get; set; }

    public int PayoutId { get; set; }
    public Payout Payout { get; set; } = null!;

    public int SaleItemId { get; set; }
    public SaleItem SaleItem { get; set; } = null!;

    public decimal AmountPaid { get; set; }
}