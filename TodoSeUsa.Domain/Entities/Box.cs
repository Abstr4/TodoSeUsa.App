namespace TodoSeUsa.Domain.Entities;

public class Box : BaseAuditableEntity
{
    public string Location { get; set; } = string.Empty;

    public virtual ICollection<Product> Products { get; set; } = [];
}