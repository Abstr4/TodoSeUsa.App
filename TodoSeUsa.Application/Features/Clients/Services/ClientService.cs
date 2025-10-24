using System.Linq.Dynamic.Core;
using TodoSeUsa.Application.Features.Clients.DTOs;
using TodoSeUsa.Application.Features.Clients.Interfaces;
using TodoSeUsa.Application.Features.Providers;

namespace TodoSeUsa.Application.Features.Clients.Services;

public class ClientService : IClientService
{
    private readonly ILogger<ClientService> _logger;
    private readonly IApplicationDbContext _context;

    public ClientService(ILogger<ClientService> logger, IApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<Result<PagedItems<ClientDto>>> GetClientsWithPagination(QueryItem request, CancellationToken cancellationToken)
    {
        try
        {
            IQueryable<Client> query = _context.Clients
                .Include(b => b.Person)
                .Include(b => b.LoanNotes)
                .Include(b => b.Sales)
                .AsQueryable()
                .AsNoTracking();

            query = query.ApplyFilter(request.Filter);
            query = query.ApplySorting(request.OrderBy, ApplyCustomSorting);

            var count = await query.CountAsync(cancellationToken);

            var clientsDtos = await query
                .Skip(request.Skip)
                .Take(request.Take)
                .Select(c => new ClientDto
                {
                    Id = c.Id,
                    FirstName = c.Person.FirstName,
                    LastName = c.Person.LastName,
                    EmailAddress = c.Person.EmailAddress,
                    PhoneNumber = c.Person.PhoneNumber,
                    Address = c.Person.Address,
                    TotalLoanNotes = c.LoanNotes.Count,
                    TotalSales = c.Sales.Count,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync(cancellationToken);

            var pagedItems = new PagedItems<ClientDto>() { Items = clientsDtos, Count = count };

            return Result.Success(pagedItems);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving paged clients.");

            return Result.Failure<PagedItems<ClientDto>>(ProviderErrors.Failure());
        }
    }

    private static IQueryable<Client>? ApplyCustomSorting(IQueryable<Client> query, string orderBy)
    {
        if (orderBy.StartsWith("TotalLoanNotes", StringComparison.OrdinalIgnoreCase))
        {
            return orderBy.EndsWith("desc", StringComparison.OrdinalIgnoreCase)
                ? query.OrderByDescending(b => b.LoanNotes.Count)
                : query.OrderBy(b => b.LoanNotes.Count);
        }

        if (orderBy.StartsWith("TotalSales", StringComparison.OrdinalIgnoreCase))
        {
            return orderBy.EndsWith("desc", StringComparison.OrdinalIgnoreCase)
                ? query.OrderByDescending(b => b.Sales.Count)
                : query.OrderBy(b => b.Sales.Count);
        }
        return null;
    }
}
