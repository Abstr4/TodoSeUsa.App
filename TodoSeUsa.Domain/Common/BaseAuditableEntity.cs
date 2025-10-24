using TodoSeUsa.Domain.Interfaces;

namespace TodoSeUsa.Domain.Common;

public abstract class BaseAuditableEntity : BaseEntity, ISoftDelete
{
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public DateTime? DeletedAt { get; set; }
}