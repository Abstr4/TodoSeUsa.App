namespace TodoSeUsa.Domain.Entities;

public class LoanedProduct : BaseAuditableEntity
{
    public int LoanNoteId { get; set; }

    public LoanNote LoanNote { get; set; } = null!;

    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;

    public LoanedProductStatus LoanedProductStatus { get; set; } = LoanedProductStatus.Active;
}