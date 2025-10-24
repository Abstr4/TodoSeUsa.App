using TodoSeUsa.Domain.Entities;

namespace TodoSeUsa.Domain.Interfaces;

public interface IPerson
{
    public int PersonId { get; set; }
    public Person Person { get; set; }
}