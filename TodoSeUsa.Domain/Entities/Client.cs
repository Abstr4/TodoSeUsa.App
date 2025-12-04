using TodoSeUsa.Domain.Interfaces;

namespace TodoSeUsa.Domain.Entities;

public class Client : BaseAuditableEntity, IPerson
{
    public int PersonId { get; set; }

    public Person Person { get; set; } = null!;

    public ICollection<Sale> Sales { get; set; } = [];

    public ICollection<LoanNote> LoanNotes { get; set; } = [];
}