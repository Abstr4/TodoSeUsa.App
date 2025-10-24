using System.Linq.Dynamic.Core;
using TodoSeUsa.Application.Features.Consignments.DTOs;
using TodoSeUsa.Application.Features.Consignments.Interfaces;

namespace TodoSeUsa.Application.Features.Consignments.Services;

public sealed class ConsignmentService : IConsignmentService
{
    private readonly ILogger<ConsignmentService> _logger;
    private readonly IApplicationDbContext _context;

    public ConsignmentService(ILogger<ConsignmentService> logger, IApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    public async Task<Result<PagedItems<ConsignmentDto>>> GetConsignmentsWithPagination(QueryItem request, CancellationToken cancellationToken)
    {
        try
        {
            IQueryable<Consignment> query = _context.Consignments
                .Include(b => b.Products)
                .Include(b => b.Provider)
                    .ThenInclude(p => p.Person)
                .AsQueryable()
                .AsNoTracking();


            query = query.ApplyFilter(request.Filter);
            query = query.ApplySorting(request.OrderBy, ApplyCustomSorting);

            var count = await query.CountAsync(cancellationToken);

            var boxesDtos = await query
                .Skip(request.Skip)
                .Take(request.Take)
                .Select(b => new ConsignmentDto
                {
                    Id = b.Id,
                    TotalProducts = b.Products.Count,
                    FirstName = b.Provider.Person.FirstName,
                    LastName = b.Provider.Person.LastName,
                    Notes = b.Notes,
                    DateIssued = b.DateIssued,
                    ProviderId = b.ProviderId,
                    CreatedAt = b.CreatedAt
                })
                .ToListAsync(cancellationToken);

            var pagedItems = new PagedItems<ConsignmentDto>() { Items = boxesDtos, Count = count };

            return Result.Success(pagedItems);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving paged boxes.");

            return Result.Failure<PagedItems<ConsignmentDto>>(ConsignmentErrors.Failure());
        }
    }

    private static IQueryable<Consignment>? ApplyCustomSorting(IQueryable<Consignment> query, string orderBy)
    {

        if (orderBy.StartsWith("TotalProducts", StringComparison.OrdinalIgnoreCase))
        {
            return orderBy.EndsWith("desc", StringComparison.OrdinalIgnoreCase)
                ? query.OrderByDescending(b => b.Products.Count)
                : query.OrderBy(b => b.Products.Count);
        }
        return null;
    }
}
