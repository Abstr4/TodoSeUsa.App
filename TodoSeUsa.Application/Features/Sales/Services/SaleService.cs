using System.Linq.Dynamic.Core;
using TodoSeUsa.Application.Features.Sales.DTOs;
using TodoSeUsa.Application.Features.Sales.Interfaces;

namespace TodoSeUsa.Application.Features.Sales.Services;

public sealed class SaleService : ISaleService
{
    private readonly ILogger<SaleService> _logger;
    private readonly IApplicationDbContext _context;
    public SaleService(ILogger<SaleService> logger, IApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<Result<PagedItems<SaleDto>>> GetSalesWithPagination(QueryItem request, CancellationToken cancellationToken)
    {
        try
        {
            IQueryable<Sale> query = _context.Sales
                .Include(s => s.Client)
                .Include(s => s.Products)
                .Include(s => s.Payments)
                .AsQueryable()
                .AsNoTracking();


            query = query.ApplyFilter(request.Filter);
            // query = query.ApplySorting(request.OrderBy, ApplyCustomSorting);

            var count = await query.CountAsync(cancellationToken);

            var boxesDtos = await query
                .Skip(request.Skip)
                .Take(request.Take)
                .Select(s => new SaleDto
                {
                    Id = s.Id,
                    Status = s.Status,
                    Method = s.Method,
                    TotalProducts = s.Products.Count,
                    TotalPayments = s.Payments.Count,
                    ClientId = s.ClientId,
                    ClientFirstName = s.Client != null ? s.Client.Person.FirstName : null,
                    ClientLastName = s.Client != null ? s.Client.Person.LastName : null,
                    DateIssued = s.DateIssued,
                    DueDate = s.DueDate,
                    Notes = s.Notes,
                    CreatedAt = s.CreatedAt
                })
                .ToListAsync(cancellationToken);

            var pagedItems = new PagedItems<SaleDto>()
            {
                Items = boxesDtos,
                Count = count
            };

            return Result.Success(pagedItems);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving paged boxes.");

            return Result.Failure<PagedItems<SaleDto>>(SaleErrors.Failure());
        }
    }

    private static IQueryable<Sale>? ApplyCustomSorting(IQueryable<Sale> query, string orderBy)
    {

        if (orderBy.StartsWith("TotalProducts", StringComparison.OrdinalIgnoreCase))
        {
            return orderBy.EndsWith("desc", StringComparison.OrdinalIgnoreCase)
                ? query.OrderByDescending(b => b.Products.Count)
                : query.OrderBy(b => b.Products.Count);
        }
        if (orderBy.StartsWith("TotalPayments", StringComparison.OrdinalIgnoreCase))
        {
            return orderBy.EndsWith("desc", StringComparison.OrdinalIgnoreCase)
                ? query.OrderByDescending(b => b.Payments.Count)
                : query.OrderBy(b => b.Payments.Count);
        }
        return null;
    }
}
