using System.Linq.Dynamic.Core;
using TodoSeUsa.Application.Common.Enums;
using TodoSeUsa.Application.Common.Helpers;
using TodoSeUsa.Application.Common.Services;
using TodoSeUsa.Application.Features.Consignments.DTOs;
using TodoSeUsa.Application.Features.Consignments.Interfaces;
using TodoSeUsa.Application.Features.Consignments.Validators;

namespace TodoSeUsa.Application.Features.Consignments.Services;

public sealed class ConsignmentService : IConsignmentService
{
    private readonly ILogger<ConsignmentService> _logger;
    private readonly IApplicationDbContextFactory _contextFactory;

    public ConsignmentService(ILogger<ConsignmentService> logger, IApplicationDbContextFactory contextFactory)
    {
        _logger = logger;
        _contextFactory = contextFactory;
    }

    public async Task<Result<PagedItems<ConsignmentDto>>> GetAllAsync(QueryRequest request, CancellationToken ct)
    {
        var _context = await _contextFactory.CreateDbContextAsync(ct);

        var query = _context.Consignments
            .Include(c => c.Provider)
                .ThenInclude(p => p.Person)
            .AsQueryable();

        if (request.Filters != null && request.Filters.Count > 0)
        {
            query = ApplyCustomFilter(query, request);
        }

        query = QueryableExtensions.ApplyCustomSorting(query, request.Sorts, QuerySortingCases.ConsignmentSorts);

        var totalCount = await query.CountAsync(ct);

        query = query.Skip(request.Skip).Take(request.Take);

        var items = await query
            .Select(c => new ConsignmentDto
            {
                Id = c.Id,
                TotalProducts = c.Products.Count,
                ProviderFirstName = c.Provider.Person.FirstName,
                ProviderLastName = c.Provider.Person.LastName,
                DateIssued = c.DateIssued,
                Notes = c.Notes,
                ProviderId = c.ProviderId,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
            })
        .ToListAsync(ct);

        return Result.Success(new PagedItems<ConsignmentDto>
        {
            Items = items,
            Count = totalCount
        });
    }

    public async Task<Result<PagedItems<ConsignmentDto>>> GetByProviderIdAsync(QueryRequest request, int providerId, CancellationToken ct)
    {
        var _context = await _contextFactory.CreateDbContextAsync(ct);

        var query = _context.Consignments
            .Include(c => c.Provider)
                .ThenInclude(p => p.Person)
            .Where(c => c.ProviderId == providerId);

        if (request.Filters != null && request.Filters.Count > 0)
        {
            query = ApplyCustomFilter(query, request);
        }

        query = QueryableExtensions.ApplyCustomSorting(query, request.Sorts, QuerySortingCases.ConsignmentSorts);

        var totalCount = await query.CountAsync(ct);

        query = query.Skip(request.Skip).Take(request.Take);

        var items = await query
            .Select(c => new ConsignmentDto
            {
                Id = c.Id,
                TotalProducts = c.Products.Count,
                ProviderFirstName = c.Provider.Person.FirstName,
                ProviderLastName = c.Provider.Person.LastName,
                DateIssued = c.DateIssued,
                Notes = c.Notes,
                ProviderId = providerId,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
            })
        .ToListAsync(ct);

        return Result.Success(new PagedItems<ConsignmentDto>
        {
            Items = items,
            Count = totalCount
        });
    }

    public async Task<Result<ConsignmentDto>> GetByIdAsync(int consignmentId, CancellationToken ct)
    {
        if (consignmentId <= 0)
            return Result.Failure<ConsignmentDto>(ConsignmentErrors.Failure("El Id debe ser mayor que cero."));

        try
        {
            var _context = await _contextFactory.CreateDbContextAsync(ct);

            var consignmentDto = await _context.Consignments
                .Where(c => c.Id == consignmentId)
                .Select(c => new ConsignmentDto
                {
                    Id = c.Id,
                    TotalProducts = c.Products.Count,
                    ProviderId = c.ProviderId,
                    ProviderFirstName = c.Provider.Person.FirstName,
                    ProviderLastName = c.Provider.Person.LastName,
                    DateIssued = c.DateIssued,
                    Notes = c.Notes,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (consignmentDto == null)
                return Result.Failure<ConsignmentDto>(ConsignmentErrors.NotFound(consignmentId));

            return Result.Success(consignmentDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to retrieve the consignment with ID {consignmentId}.", consignmentId);
            return Result.Failure<ConsignmentDto>(ConsignmentErrors.Failure("Ocurrió un error inesperado al intentar recuperar la consignación."));
        }
    }

    public async Task<Result<int>> CreateAsync(CreateConsignmentDto createConsignmentDto, CancellationToken ct)
    {
        var validator = new CreateConsignmentDtoValidator();
        var validationResult = await validator.ValidateAsync(createConsignmentDto, ct);

        if (!validationResult.IsValid)
            return Result.Failure<int>(ConsignmentErrors.Failure(validationResult.ToString()));

        Consignment consignment = new()
        {
            ProviderId = createConsignmentDto.ProviderId,
            DateIssued = createConsignmentDto.DateIssued,
            Notes = createConsignmentDto.Notes,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        try
        {
            var _context = await _contextFactory.CreateDbContextAsync(ct);

            var entry = await _context.Consignments.AddAsync(consignment, ct);

            var saved = await _context.SaveChangesAsync(ct);

            if (saved > 0)
                return Result.Success(entry.Entity.Id);

            return Result.Failure<int>(ConsignmentErrors.Failure("No se pudo crear la consignación."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to create the consignment.");
            return Result.Failure<int>(ConsignmentErrors.Failure($"Ocurrió un error inesperado al intentar crear la consignación."));
        }
    }

    public async Task<Result<bool>> DeleteByIdAsync(int consignmentId, CancellationToken ct)
    {
        if (consignmentId <= 0)
            return Result.Failure<bool>(ConsignmentErrors.Failure("El Id debe ser mayor que cero."));

        try
        {
            var _context = await _contextFactory.CreateDbContextAsync(ct);

            var consignment = await _context.Consignments
                .Include(b => b.Products)
                .FirstOrDefaultAsync(b => b.Id == consignmentId, ct);

            if (consignment is null)
                return Result.Failure<bool>(ConsignmentErrors.NotFound(consignmentId));

            if (consignment.Products.Count > 0)
            {
                return Result.Failure<bool>(ConsignmentErrors.Failure("No se puede borrar una consignación que contiene productos."));
            }

            _context.Consignments.Remove(consignment);
            await _context.SaveChangesAsync(ct);
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to delete the consignment with ID {consignmentId}", consignmentId);
            return Result.Failure<bool>(ConsignmentErrors.Failure($"Ocurrió un error inesperado al intentar borrar la consignación."));
        }
    }

    public async Task<Result<bool>> EditByIdAsync(int consignmentId, EditConsignmentDto editConsignmentDto, CancellationToken ct)
    {
        if (consignmentId <= 0)
            return Result.Failure<bool>(ConsignmentErrors.Failure("El Id debe ser mayor que cero."));

        var validator = new EditConsignmentDtoValidator();
        var validationResult = await validator.ValidateAsync(editConsignmentDto, ct);

        if (!validationResult.IsValid)
            return Result.Failure<bool>(ConsignmentErrors.Failure(validationResult.ToString()));

        try
        {
            var _context = await _contextFactory.CreateDbContextAsync(ct);

            Consignment? consignment = await _context.Consignments.FirstOrDefaultAsync(b => b.Id == consignmentId, ct);
            if (consignment == null)
            {
                return Result.Failure<bool>(ConsignmentErrors.NotFound(consignmentId));
            }
            consignment.DateIssued = editConsignmentDto.DateIssued;
            consignment.Notes = editConsignmentDto.Notes;
            consignment.ProviderId = editConsignmentDto.ProviderId;

            await _context.SaveChangesAsync(ct);
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to edit the consignment with ID {consignmentId}.", consignmentId);
            return Result.Failure<bool>(ConsignmentErrors.Failure($"Ocurrió un error inesperado al intentar editar la consignación."));
        }
    }

    public static IQueryable<Consignment> ApplyCustomFilter(IQueryable<Consignment> query, QueryRequest request)
    {
        if (request.Filters == null || request.Filters.Count == 0)
            return query;

        var remainingFilters = new List<FilterDescriptor>();

        foreach (var filter in request.Filters)
        {
            if (string.IsNullOrWhiteSpace(filter.Property) || filter.FilterValue == null)
                continue;

            switch (filter.Property)
            {
                case "ProviderFullName":
                    var val = filter.FilterValue.ToString();
                    query = query.Where(c =>
                        EF.Functions.Like(c.Provider.Person.FirstName, $"%{val}%") ||
                        EF.Functions.Like(c.Provider.Person.LastName, $"%{val}%")
                    // || EF.Functions.Like(EF.Property<int>(c, "ProviderId").ToString(), $"%{val}%")
                    );
                    break;

                default:
                    remainingFilters.Add(filter);
                    break;
            }
        }

        if (remainingFilters.Count > 0)
        {
            var subRequest = new QueryRequest
            {
                Filters = remainingFilters,
                LogicalFilterOperator = request.LogicalFilterOperator
            };
            var predicate = PredicateBuilder.BuildPredicate<Consignment>(subRequest);
            query = query.Where(predicate);
        }

        return query;
    }
}