using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using TodoSeUsa.Application.Common.Enums;
using TodoSeUsa.Application.Common.Services;
using TodoSeUsa.Application.Features.Consignments.DTOs;
using TodoSeUsa.Application.Features.Consignments.Interfaces;
using TodoSeUsa.Application.Features.Consignments.Validators;
using TodoSeUsa.Domain.Enums;

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

    public async Task<Result<PagedItems<ConsignmentDto>>> GetAllAsync(QueryRequest request, CancellationToken cancellationToken)
    {
        var query = _context.Consignments
            .Include(c => c.Provider)
                .ThenInclude(p => p.Person)
            .AsQueryable();

        if (request.Filters != null && request.Filters.Count > 0)
        {
            query = ApplyCustomFilter(query, request);
        }
        if (request.Sorts != null && request.Sorts.Count > 0)
        {
            query = ApplyCustomSorting(query, request.Sorts);
        }
        else
        {
            query = query.OrderBy(x => x.Id);
        }

        var totalCount = await query.CountAsync(cancellationToken);

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
        .ToListAsync(cancellationToken);

        return Result.Success(new PagedItems<ConsignmentDto>
        {
            Items = items,
            Count = totalCount
        });
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
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        try
        {
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

    public async Task<Result<ConsignmentDto>> GetByIdAsync(int consignmentId, CancellationToken cancellationToken)
    {
        if (consignmentId <= 0)
            return Result.Failure<ConsignmentDto>(ConsignmentErrors.Failure("El Id debe ser mayor que cero."));

        try
        {
            var consignment = await _context.Consignments
                .Include(c => c.Products)
                .Include(c => c.Provider)
                    .ThenInclude(p => p.Person)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == consignmentId, cancellationToken);
            if (consignment == null)
            {
                return Result.Failure<ConsignmentDto>(ConsignmentErrors.NotFound(consignmentId));
            }
            var consignmentDto = new ConsignmentDto
            {
                Id = consignment.Id,
                TotalProducts = consignment.Products.Count,
                ProviderFirstName = consignment.Provider.Person.FirstName,
                ProviderLastName = consignment.Provider.Person.LastName,
                DateIssued = consignment.DateIssued,
                Notes = consignment.Notes,
                ProviderId = consignment.ProviderId,
                CreatedAt = consignment.CreatedAt,
                UpdatedAt = consignment.UpdatedAt
            };
            return Result.Success(consignmentDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to retrieve the consignment with ID {consignmentId}.", consignmentId);
            return Result.Failure<ConsignmentDto>(ConsignmentErrors.Failure($"Ocurrió un error inesperado al intentar recuperar la consignación."));
        }
    }

    public async Task<Result<bool>> DeleteByIdAsync(int consignmentId, CancellationToken cancellationToken)
    {
        if (consignmentId <= 0)
            return Result.Failure<bool>(ConsignmentErrors.Failure("El Id debe ser mayor que cero."));

        try
        {
            var consignment = await _context.Consignments
                .Include(b => b.Products)
                .FirstOrDefaultAsync(b => b.Id == consignmentId, cancellationToken);

            if (consignment is null)
                return Result.Failure<bool>(ConsignmentErrors.NotFound(consignmentId));

            if (consignment.Products.Count > 0)
            {
                return Result.Failure<bool>(ConsignmentErrors.Failure("No se puede borrar una consignación que contiene productos."));
            }

            _context.Consignments.Remove(consignment);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to delete the consignment with ID {consignmentId}", consignmentId);
            return Result.Failure<bool>(ConsignmentErrors.Failure($"Ocurrió un error inesperado al intentar borrar la consignación."));
        }
    }

    public async Task<Result<bool>> EditByIdAsync(int consignmentId, EditConsignmentDto editConsignmentDto, CancellationToken cancellationToken)
    {
        if (consignmentId <= 0)
            return Result.Failure<bool>(ConsignmentErrors.Failure("El Id debe ser mayor que cero."));

        var validator = new EditConsignmentDtoValidator();
        var validationResult = await validator.ValidateAsync(editConsignmentDto, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Failure<bool>(ConsignmentErrors.Failure(validationResult.ToString()));

        try
        {
            Consignment? consignment = await _context.Consignments.FirstOrDefaultAsync(b => b.Id == consignmentId, cancellationToken);
            if (consignment == null)
            {
                return Result.Failure<bool>(ConsignmentErrors.NotFound(consignmentId));
            }
            consignment.DateIssued = editConsignmentDto.DateIssued;
            consignment.Notes = editConsignmentDto.Notes;
            consignment.ProviderId = editConsignmentDto.ProviderId;

            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to edit the consignment with ID {consignmentId}.", consignmentId);
            return Result.Failure<bool>(ConsignmentErrors.Failure($"Ocurrió un error inesperado al intentar editar la consignación."));
        }
    }

    public static IQueryable<Consignment> ApplyCustomSorting(IQueryable<Consignment> query, IEnumerable<SortDescriptor>? sorts)
    {
        var sort = sorts?.FirstOrDefault();
        if (sort == null || string.IsNullOrWhiteSpace(sort.Property))
            return query;

        var property = sort.Property;
        var isDescending = sort.SortOrder == SortOrder.Descending;

        if (property.Equals("TotalProducts", StringComparison.OrdinalIgnoreCase))
        {
            return isDescending
                ? query.OrderByDescending(b => b.Products.Count)
                : query.OrderBy(b => b.Products.Count);
        }
        if (sort.Property == "ProviderFullName")
        {
            return isDescending
                 ? query.OrderByDescending(c => c.Provider.Person.FirstName).ThenByDescending(c => c.Provider.Person.LastName)
                 : query.OrderBy(c => c.Provider.Person.FirstName).ThenBy(c => c.Provider.Person.LastName);
        }

        return isDescending
            ? query.OrderByDescending(e => EF.Property<object>(e, property))
            : query.OrderBy(e => EF.Property<object>(e, property));

    }

    public static IQueryable<Consignment> ApplyCustomFilter(
        IQueryable<Consignment> query,
        QueryRequest request)
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
