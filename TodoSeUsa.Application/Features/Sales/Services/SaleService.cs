using System.Linq.Dynamic.Core;
using TodoSeUsa.Application.Features.Sales.DTOs;
using TodoSeUsa.Application.Features.Sales.Interfaces;

namespace TodoSeUsa.Application.Features.Sales.Services;

public sealed class SaleService : ISaleService
{
    private readonly ILogger<SaleService> _logger;
    private readonly IApplicationDbContextFactory _contextFactory;

    public SaleService(ILogger<SaleService> logger, IApplicationDbContextFactory contextFactory)
    {
        _logger = logger;
        _contextFactory = contextFactory;
    }

    public async Task<Result<PagedItems<SaleDto>>> GetSalesWithPagination(QueryItem request, CancellationToken ct)
    {
        try
        {
            var _context = await _contextFactory.CreateDbContextAsync(ct);

            IQueryable<Sale> query = _context.Sales
                .AsQueryable()
                .AsNoTracking();

            query = query.ApplyFilter(request.Filter);

            var count = await query.CountAsync(ct);

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
                .ToListAsync(ct);

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
}