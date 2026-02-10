namespace TodoSeUsa.Domain.Entities;

public class Consignment : BaseAuditableEntity
{
    public string Code { get; set; } = string.Empty;

    public DateTime DateIssued { get; set; } = DateTime.Now;

    public string? Notes { get; set; } = string.Empty;

    public int ConsignorId { get; set; }

    public Consignor Consignor { get; set; } = null!;

    public ICollection<Product> Products { get; set; } = [];
}