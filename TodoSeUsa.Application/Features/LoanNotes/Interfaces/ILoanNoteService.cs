using TodoSeUsa.Application.Features.LoanNotes.DTOs;

namespace TodoSeUsa.Application.Features.LoanNotes.Interfaces;

public interface ILoanNoteService
{
    Task<Result<PagedItems<LoanNoteDto>>> GetLoanNotesWithPagination(QueryItem request, CancellationToken cancellationToken);
}
