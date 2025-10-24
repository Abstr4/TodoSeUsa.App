namespace TodoSeUsa.Domain.Interfaces;

public interface ISoftDelete
{
    public DateTime? DeletedAt { get; set; }
}