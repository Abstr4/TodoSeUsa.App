namespace TodoSeUsa.Domain.Common;

public abstract class BaseEntity
{
    public int Id { get; set; }

    public Guid PublicIdentifier { get; set; } = Guid.NewGuid();
}