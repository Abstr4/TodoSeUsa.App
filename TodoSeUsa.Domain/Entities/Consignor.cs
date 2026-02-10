using TodoSeUsa.Domain.Interfaces;

namespace TodoSeUsa.Domain.Entities;

public class Consignor : BaseAuditableEntity, IPerson
{
    public decimal CommissionPercent { get; set; } = 0m;

    public int PersonId { get; set; }

    public Person Person { get; set; } = null!;

    public ICollection<Consignment> Consignments { get; set; } = [];
}