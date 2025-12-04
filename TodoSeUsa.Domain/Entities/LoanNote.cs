namespace TodoSeUsa.Domain.Entities;

public class LoanNote : BaseAuditableEntity
{
    public LoanNoteStatus Status { get; set; } = LoanNoteStatus.Active;

    public DateTime LoanDate { get; set; }

    public DateTime ExpectedReturnDate { get; set; }

    public int ClientId { get; set; }

    public Client Client { get; set; } = null!;

    public ICollection<LoanedProduct> LoanedProducts { get; set; } = [];
}