namespace TodoSeUsa.Domain.Entities;

public class Reservation : BaseAuditableEntity
{
    public DateTime DateIssued { get; set; }

    public DateTime ExpiresAt { get; set; }

    public ReservationStatus Status { get; set; } = ReservationStatus.Active;

    public int ClientId { get; set; }

    public Client Client { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = [];
}