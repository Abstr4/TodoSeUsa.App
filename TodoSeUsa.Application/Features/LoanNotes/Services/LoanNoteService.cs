using System.Linq.Dynamic.Core;
using TodoSeUsa.Application.Features.LoanNotes.DTOs;
using TodoSeUsa.Application.Features.LoanNotes.Interfaces;

namespace TodoSeUsa.Application.Features.LoanNotes.Services;

public sealed class LoanNoteService : ILoanNoteService
{
    private readonly ILogger<LoanNoteService> _logger;
    private readonly IApplicationDbContext _context;

    public LoanNoteService(ILogger<LoanNoteService> logger, IApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<Result<PagedItems<LoanNoteDto>>> GetLoanNotesWithPagination(QueryItem request, CancellationToken cancellationToken)
    {
        try
        {
            IQueryable<LoanNote> query = _context.LoanNotes
                .Include(ln => ln.LoanedProducts)
                .AsQueryable()
                .AsNoTracking();

            query = query.ApplyFilter(request.Filter);
            query = ApplySorting(query, request.OrderBy);

            var count = await query.CountAsync(cancellationToken);

            var loanNotesDtos = await query
                .Skip(request.Skip)
                .Take(request.Take)
                .Select(ln => new LoanNoteDto
                {
                    Id = ln.Id,
                    Status = ln.Status,
                    TotalLoanedProducts = ln.LoanedProducts.Count,
                    LoanDate = ln.LoanDate,
                    ExpectedReturnDate = ln.ExpectedReturnDate,
                    ClientId = ln.ClientId,
                    CreatedAt = ln.CreatedAt
                })
                .ToListAsync(cancellationToken);

            var pagedItems = new PagedItems<LoanNoteDto>()
            {
                Items = loanNotesDtos,
                Count = count
            };

            return Result.Success(pagedItems);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving paged loanNotes.");

            return Result.Failure<PagedItems<LoanNoteDto>>(LoanNoteErrors.Failure());
        }
    }

    private static IQueryable<LoanNote> ApplySorting(IQueryable<LoanNote> query, string? orderBy)
    {
        if (string.IsNullOrWhiteSpace(orderBy))
            return query.OrderBy(b => b.Id);

        var customSortedQuery = ApplyCustomSorting(query, orderBy);
        if (customSortedQuery is not null)
            return customSortedQuery;

        return query.OrderBy(orderBy);
    }

    private static IQueryable<LoanNote>? ApplyCustomSorting(IQueryable<LoanNote> query, string orderBy)
    {
        if (orderBy.StartsWith("TotalLoanedProducts", StringComparison.OrdinalIgnoreCase))
        {
            return orderBy.EndsWith("desc", StringComparison.OrdinalIgnoreCase)
                ? query.OrderByDescending(ln => ln.LoanedProducts.Count)
                : query.OrderBy(ln => ln.LoanedProducts.Count);
        }
        return null;
    }
}
