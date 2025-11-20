using System.Linq.Dynamic.Core;
using TodoSeUsa.Application.Common.Enums;
using TodoSeUsa.Application.Common.Errors;
using TodoSeUsa.Application.Common.Services;
using TodoSeUsa.Application.Features.Providers.DTOs;
using TodoSeUsa.Application.Features.Providers.Interfaces;
using TodoSeUsa.Application.Features.Providers.Validators;

namespace TodoSeUsa.Application.Features.Providers.Services;

public class ProviderService : IProviderService
{
    private readonly ILogger<ProviderService> _logger;
    private readonly IApplicationDbContext _context;
    private readonly IPersonService _personService;

    public ProviderService(ILogger<ProviderService> logger, IApplicationDbContext context, IPersonService personService)
    {
        _logger = logger;
        _context = context;
        _personService = personService;
    }

    public async Task<Result<PagedItems<ProviderDto>>> GetAllAsync(QueryRequest request, CancellationToken cancellationToken)
    {
        var query = _context.Providers
            .Include(c => c.Consignments)
            .Include(c => c.Person)
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
            .Select(c => new ProviderDto
            {
                Id = c.Id,
                CommissionPercent = c.CommissionPercent,
                TotalConsignments = c.Consignments.Count,
                FirstName = c.Person.FirstName,
                LastName = c.Person.LastName,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
        .ToListAsync(cancellationToken);

        return Result.Success(new PagedItems<ProviderDto>
        {
            Items = items,
            Count = totalCount
        });
    }

    public async Task<Result<int>> CreateAsync(CreateProviderDto createProviderDto, CancellationToken cancellationToken)
    {
        var dtoValidator = new CreateProviderDtoValidator();
        var dtoValidationResult = await dtoValidator.ValidateAsync(createProviderDto, cancellationToken);

        if (!dtoValidationResult.IsValid)
            return Result.Failure<int>(ProviderErrors.Failure(dtoValidationResult.ToString()));

        try
        {
            var person = await _personService.GetByContactInfoAsync(
                createProviderDto.EmailAddress,
                createProviderDto.PhoneNumber,
                cancellationToken);

            if (person == null)
            {
                var newPerson = new Person
                {
                    FirstName = createProviderDto.FirstName,
                    LastName = createProviderDto.LastName,
                    EmailAddress = createProviderDto.EmailAddress,
                    PhoneNumber = createProviderDto.PhoneNumber,
                    Address = createProviderDto.Address
                };

                try
                {
                    person = await _personService.CreateAsync(newPerson, cancellationToken);
                }
                catch (ValidationException ex)
                {
                    return Result.Failure<int>(PersonErrors.Failure(ex.Message));
                }
            }

            var existingProvider = await _context.Providers
                .AnyAsync(p => p.PersonId == person.Id, cancellationToken);

            if (existingProvider)
                return Result.Failure<int>(ProviderErrors.Failure("Esta persona ya es un proveedor."));

            var provider = new Provider
            {
                CommissionPercent = createProviderDto.CommissionPercent,
                PersonId = person.Id
            };

            var entry = await _context.Providers.AddAsync(provider, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success(entry.Entity.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to create the provider.");
            return Result.Failure<int>(ProviderErrors.Failure($"Ocurrió un error inesperado al intentar crear el proveedor."));
        }

    }

    public async Task<Result<ProviderDto>> GetByIdAsync(int providerId, CancellationToken cancellationToken)
    {
        if (providerId <= 0)
            return Result.Failure<ProviderDto>(ProviderErrors.Failure("El Id debe ser mayor que cero."));

        try
        {
            var provider = await _context.Providers
                    .Include(p => p.Person)
                    .Include(p => p.Consignments)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == providerId, cancellationToken);

            if (provider == null)
            {
                return Result.Failure<ProviderDto>(ProviderErrors.NotFound(providerId));
            }

            var providerDto = new ProviderDto
            {
                Id = provider.Id,
                FirstName = provider.Person.FirstName,
                LastName = provider.Person.LastName,
                EmailAddress = provider.Person.EmailAddress,
                PhoneNumber = provider.Person.PhoneNumber,
                Address = provider.Person.Address,
                CommissionPercent = provider.CommissionPercent,
                TotalConsignments = provider.Consignments.Count,
                CreatedAt = provider.CreatedAt,
                UpdatedAt = provider.UpdatedAt
            };
            return Result.Success(providerDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to retrieve the provider with ID {providerId}.", providerId);
            return Result.Failure<ProviderDto>(ProviderErrors.Failure($"Ocurrió un error inesperado al intentar recuperar el proveedor."));
        }
    }

    public async Task<Result<bool>> DeleteByIdAsync(int providerId, CancellationToken cancellationToken)
    {
        if (providerId <= 0)
            return Result.Failure<bool>(ProviderErrors.Failure("El Id debe ser mayor que cero."));

        try
        {
            var provider = await _context.Providers
                .Include(b => b.Consignments)
                .FirstOrDefaultAsync(b => b.Id == providerId, cancellationToken);

            if (provider is null)
                return Result.Failure<bool>(ProviderErrors.NotFound(providerId));

            if (provider.Consignments.Count > 0)
                return Result.Failure<bool>(ProviderErrors.Failure(
                    "No se puede borrar una consignación que contiene productos."
                ));

            _context.Providers.Remove(provider);
            await _context.SaveChangesAsync(cancellationToken);

            await _personService.DeletePersonIfNoRolesAsync(provider.PersonId, cancellationToken);

            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "An error occurred while trying to delete the provider with ID {providerId}",
                providerId);

            return Result.Failure<bool>(ProviderErrors.Failure(
                "Ocurrió un error inesperado al intentar borrar el proveedor."
            ));
        }
    }

    public async Task<Result<bool>> EditByIdAsync(int providerId, EditProviderDto dto, CancellationToken cancellationToken)
    {
        if (providerId <= 0)
            return Result.Failure<bool>(ProviderErrors.Failure("El Id debe ser mayor que cero."));

        var dtoValidator = new EditProviderDtoValidator();
        var dtoValidation = await dtoValidator.ValidateAsync(dto, cancellationToken);

        if (!dtoValidation.IsValid)
            return Result.Failure<bool>(ProviderErrors.Failure(dtoValidation.ToString()));

        try
        {
            var provider = await _context.Providers
                .Include(p => p.Person)
                .FirstOrDefaultAsync(p => p.Id == providerId, cancellationToken);

            if (provider == null)
                return Result.Failure<bool>(ProviderErrors.NotFound(providerId));

            provider.CommissionPercent = dto.CommissionPercent;

            provider.Person.FirstName = dto.FirstName;
            provider.Person.LastName = dto.LastName;
            provider.Person.EmailAddress = dto.EmailAddress;
            provider.Person.PhoneNumber = dto.PhoneNumber;
            provider.Person.Address = dto.Address;

            try
            {
                await _personService.UpdateAsync(provider.Person, cancellationToken);
            }
            catch (ValidationException ex)
            {
                return Result.Failure<bool>(PersonErrors.Failure(ex.Message));
            }

            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error editing provider {providerId}", providerId);
            return Result.Failure<bool>(ProviderErrors.Failure("Ocurrió un error inesperado."));
        }
    }

    public static IQueryable<Provider> ApplyCustomSorting(IQueryable<Provider> query, IEnumerable<SortDescriptor>? sorts)
    {
        var sort = sorts?.FirstOrDefault();
        if (sort == null || string.IsNullOrWhiteSpace(sort.Property))
            return query;

        var property = sort.Property;
        var isDescending = sort.SortOrder == SortOrder.Descending;

        if (property.Equals("TotalConsignments", StringComparison.OrdinalIgnoreCase))
        {
            return isDescending
                ? query.OrderByDescending(b => b.Consignments.Count)
                : query.OrderBy(b => b.Consignments.Count);
        }
        if (sort.Property == "FullName")
        {
            return isDescending
                 ? query.OrderByDescending(c => c.Person.FirstName)
                            .ThenByDescending(c => c.Person.LastName)
                 : query.OrderBy(c => c.Person.FirstName).ThenBy(c => c.Person.LastName);
        }

        return isDescending
            ? query.OrderByDescending(e => EF.Property<object>(e, property))
            : query.OrderBy(e => EF.Property<object>(e, property));

    }

    public static IQueryable<Provider> ApplyCustomFilter(IQueryable<Provider> query, QueryRequest request)
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
                case "FullName":
                    var val = filter.FilterValue.ToString();
                    query = query.Where(c =>
                        EF.Functions.Like(c.Person.FirstName, $"%{val}%") ||
                        EF.Functions.Like(c.Person.LastName, $"%{val}%") 
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
            var predicate = PredicateBuilder.BuildPredicate<Provider>(subRequest);
            query = query.Where(predicate);
        }

        return query;
    }
}
