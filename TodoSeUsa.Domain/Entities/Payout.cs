namespace TodoSeUsa.Domain.Entities;

public class Payout
{
    public int Id { get; set; }

    public int ConsignorId { get; set; }
    public Consignor Consignor { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public decimal TotalAmount { get; set; }

    public ICollection<PayoutLine> Lines { get; set; } = [];
}
