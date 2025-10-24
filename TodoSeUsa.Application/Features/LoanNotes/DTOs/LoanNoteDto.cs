using TodoSeUsa.Domain.Enums;

namespace TodoSeUsa.Application.Features.LoanNotes.DTOs;

public class LoanNoteDto
{
    public int Id { get; init; }

    public LoanNoteStatus Status { get; init; }

    public int TotalLoanedProducts { get; init; }

    public DateTime LoanDate { get; init; }

    public DateTime ExpectedReturnDate { get; init; }

    public int ClientId { get; init; }

    public DateTime CreatedAt { get; init; }
}
