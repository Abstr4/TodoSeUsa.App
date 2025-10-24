using TodoSeUsa.Domain.Interfaces;

namespace TodoSeUsa.Domain.Entities;

public class Client : BaseAuditableEntity, IPerson
{
    public int PersonId { get; set; }

    public Person Person { get; set; } = null!;

    public virtual ICollection<Sale> Sales { get; set; } = [];

    public virtual ICollection<LoanNote> LoanNotes { get; set; } = [];
}