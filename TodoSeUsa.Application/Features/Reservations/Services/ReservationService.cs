using System.Linq.Dynamic.Core;
using TodoSeUsa.Application.Features.Reservations.DTOs;
using TodoSeUsa.Application.Features.Reservations.Interfaces;

namespace TodoSeUsa.Application.Features.Reservations.Services;

public sealed class ReservationService : IReservationService
{
    private readonly ILogger<ReservationService> _logger;
    private readonly IApplicationDbContext _context;

    public ReservationService(ILogger<ReservationService> logger, IApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    public async Task<Result<PagedItems<ReservationDto>>> GetReservationsWithPagination(QueryItem request, CancellationToken cancellationToken)
    {
        try
        {
            IQueryable<Reservation> query = _context.Reservations
                .Include(b => b.Products)
                .Include(b => b.Client)
                    .ThenInclude(p => p.Person)
                .AsQueryable()
                .AsNoTracking();


            query = query.ApplyFilter(request.Filter);
            query = query.ApplySorting(request.OrderBy, ApplyCustomSorting);

            var count = await query.CountAsync(cancellationToken);

            var boxesDtos = await query
                .Skip(request.Skip)
                .Take(request.Take)
                .Select(r => new ReservationDto
                {
                    Id = r.Id,
                    TotalProducts = r.Products.Count,
                    FirstName = r.Client.Person.FirstName,
                    LastName = r.Client.Person.LastName,
                    DateIssued = r.DateIssued,
                    ExpiresAt = r.ExpiresAt,
                    Status = r.Status,
                    ClientId = r.ClientId,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync(cancellationToken);

            var pagedItems = new PagedItems<ReservationDto>()
            {
                Items = boxesDtos,
                Count = count
            };

            return Result.Success(pagedItems);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving paged boxes.");

            return Result.Failure<PagedItems<ReservationDto>>(ReservationErrors.Failure());
        }
    }

    private static IQueryable<Reservation>? ApplyCustomSorting(IQueryable<Reservation> query, string orderBy)
    {

        if (orderBy.StartsWith("TotalProducts", StringComparison.OrdinalIgnoreCase))
        {
            return orderBy.EndsWith("desc", StringComparison.OrdinalIgnoreCase)
                ? query.OrderByDescending(r => r.Products.Count)
                : query.OrderBy(r => r.Products.Count);
        }
        return null;
    }
}
