namespace TodoSeUsa.Domain.Entities;

public class Sale : BaseAuditableEntity
{
    public PaymentStatus Status { get; set; }

    public PaymentMethod Method { get; set; }

    public ICollection<Product> Products { get; set; } = [];

    public ICollection<Payment> Payments { get; set; } = [];

    public DateTime DateIssued { get; set; } = DateTime.Now;

    public DateTime DueDate { get; set; } = DateTime.Now.AddDays(30);

    public string Notes { get; set; } = string.Empty;

    public int? ClientId { get; set; }

    public Client? Client { get; set; }
}