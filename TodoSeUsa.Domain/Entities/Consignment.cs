namespace TodoSeUsa.Domain.Entities;

public class Consignment : BaseAuditableEntity
{
    public DateTime DateIssued { get; set; } = DateTime.Now;

    public string? Notes { get; set; } = string.Empty;

    public int ProviderId { get; set; }

    public Provider Provider { get; set; } = null!;

    public ICollection<Product> Products { get; set; } = [];
}