namespace TodoSeUsa.Domain.Common;

public abstract class BaseEntity
{
    public int Id { get; private set; }

    public Guid PublicIdentifier { get; private set; } = Guid.NewGuid();
}