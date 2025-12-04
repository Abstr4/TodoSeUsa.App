namespace TodoSeUsa.Domain.Entities;

public class Box : BaseAuditableEntity
{
    public string Location { get; set; } = string.Empty;

    public ICollection<Product> Products { get; set; } = [];
}