namespace TodoSeUsa.Domain.Entities;

public class ProductImage : BaseAuditableEntity
{
    public string Path { get; set; } = string.Empty;
    public bool IsMain { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
}
