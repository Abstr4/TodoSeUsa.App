using System.Linq.Dynamic.Core;
using TodoSeUsa.Application.Features.Providers.DTOs;
using TodoSeUsa.Application.Features.Providers.Interfaces;

namespace TodoSeUsa.Application.Features.Providers.Services;

public class ProviderService : IProviderService
{
    private readonly ILogger<ProviderService> _logger;
    private readonly IApplicationDbContext _context;

    public ProviderService(ILogger<ProviderService> logger, IApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<Result<PagedItems<ProviderDto>>> GetProvidersWithPagination(QueryItem request, CancellationToken cancellationToken)
    {
        try
        {
            IQueryable<Provider> query = _context.Providers
                .Include(b => b.Person)
                .Include(b => b.Consignments)
                .AsQueryable()
                .AsNoTracking();

            query = query.ApplyFilter(request.Filter);
            query = query.ApplySorting(request.OrderBy, ApplyCustomSorting);

            var count = await query.CountAsync(cancellationToken);

            var providersDtos = await query
                .Skip(request.Skip)
                .Take(request.Take)
                .Select(c => new ProviderDto
                {
                    Id = c.Id,
                    CommissionPercent = c.CommissionPercent,
                    FirstName = c.Person.FirstName,
                    LastName = c.Person.LastName,
                    TotalConsignments = c.Consignments.Count,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync(cancellationToken);

            var pagedItems = new PagedItems<ProviderDto>()
            {
                Items = providersDtos,
                Count = count
            };

            return Result.Success(pagedItems);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving paged providers.");

            return Result.Failure<PagedItems<ProviderDto>>(ProviderErrors.Failure());
        }
    }

    private static IQueryable<Provider>? ApplyCustomSorting(IQueryable<Provider> query, string orderBy)
    {
        if (orderBy.StartsWith("TotalConsignments", StringComparison.OrdinalIgnoreCase))
        {
            return orderBy.EndsWith("desc", StringComparison.OrdinalIgnoreCase)
                ? query.OrderByDescending(b => b.Consignments.Count)
                : query.OrderBy(b => b.Consignments.Count);
        }
        return null;
    }
}
