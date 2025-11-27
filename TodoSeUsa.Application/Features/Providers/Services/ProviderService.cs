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
    private readonly IApplicationDbContextFactory _contextFactory;
    private readonly IPersonService _personService;

    public ProviderService(ILogger<ProviderService> logger, IApplicationDbContextFactory contextFactory, IPersonService personService)
    {
        _logger = logger;
        _contextFactory = contextFactory;
        _personService = personService;
    }

    public async Task<Result<PagedItems<ProviderDto>>> GetAllAsync(QueryRequest request, CancellationToken ct)
    {
        var _context = await _contextFactory.CreateDbContextAsync(ct);

        var query = _context.Providers
            .Include(p => p.Person)
            .Include(p => p.Consignments)
                .ThenInclude(cg => cg.Products)
            .AsNoTracking()
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

        var totalCount = await query.CountAsync(ct);

        query = query.Skip(request.Skip).Take(request.Take);

        var items = await query
            .Select(p => new ProviderDto
            {
                Id = p.Id,
                CommissionPercent = p.CommissionPercent,
                TotalConsignments = p.Consignments.Count,
                TotalProducts = p.Consignments.Sum(cg => cg.Products.Count),
                FirstName = p.Person.FirstName,
                LastName = p.Person.LastName,
                EmailAddress = p.Person.EmailAddress,
                PhoneNumber = p.Person.PhoneNumber,
                Address = p.Person.Address,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
        .ToListAsync(ct);

        return Result.Success(new PagedItems<ProviderDto>
        {
            Items = items,
            Count = totalCount
        });
    }

    public async Task<Result<int>> CreateAsync(CreateProviderDto createProviderDto, CancellationToken ct)
    {
        var dtoValidator = new CreateProviderDtoValidator();
        var dtoValidationResult = await dtoValidator.ValidateAsync(createProviderDto, ct);

        if (!dtoValidationResult.IsValid)
            return Result.Failure<int>(ProviderErrors.Failure(dtoValidationResult.ToString()));

        try
        {
            var _context = await _contextFactory.CreateDbContextAsync(ct);

            var person = await _personService.GetByContactInfoAsync(
                createProviderDto.EmailAddress,
                createProviderDto.PhoneNumber,
                ct);

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
                    person = await _personService.CreateAsync(newPerson, ct);
                }
                catch (ValidationException ex)
                {
                    return Result.Failure<int>(PersonErrors.Failure(ex.Message));
                }
            }

            var existingProvider = await _context.Providers
                .AnyAsync(p => p.PersonId == person.Id, ct);

            if (existingProvider)
                return Result.Failure<int>(ProviderErrors.Failure("Esta persona ya es un proveedor."));

            var provider = new Provider
            {
                CommissionPercent = createProviderDto.CommissionPercent,
                PersonId = person.Id
            };

            var entry = await _context.Providers.AddAsync(provider, ct);
            await _context.SaveChangesAsync(ct);

            return Result.Success(entry.Entity.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to create the provider.");
            return Result.Failure<int>(ProviderErrors.Failure($"Ocurrió un error inesperado al intentar crear el proveedor."));
        }
    }

    public async Task<Result<ProviderDto>> GetByIdAsync(int providerId, CancellationToken ct)
    {
        if (providerId <= 0)
            return Result.Failure<ProviderDto>(ProviderErrors.Failure("El Id debe ser mayor que cero."));

        try
        {
            var _context = await _contextFactory.CreateDbContextAsync(ct);

            var providerDto = await _context.Providers
                .Where(p => p.Id == providerId)
                .Select(p => new ProviderDto
                {
                    Id = p.Id,
                    CommissionPercent = p.CommissionPercent,
                    TotalConsignments = p.Consignments.Count,
                    TotalProducts = p.Consignments.Sum(cg => cg.Products.Count),
                    FirstName = p.Person.FirstName,
                    LastName = p.Person.LastName,
                    EmailAddress = p.Person.EmailAddress,
                    PhoneNumber = p.Person.PhoneNumber,
                    Address = p.Person.Address,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (providerDto == null)
                return Result.Failure<ProviderDto>(ProviderErrors.NotFound(providerId));

            return Result.Success(providerDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to retrieve the provider with ID {providerId}.", providerId);
            return Result.Failure<ProviderDto>(ProviderErrors.Failure("Ocurrió un error inesperado al intentar recuperar el proveedor."));
        }
    }

    public async Task<Result<bool>> DeleteByIdAsync(int providerId, CancellationToken ct)
    {
        if (providerId <= 0)
            return Result.Failure<bool>(ProviderErrors.Failure("El Id debe ser mayor que cero."));

        try
        {
            var _context = await _contextFactory.CreateDbContextAsync(ct);

            var provider = await _context.Providers
                .FirstOrDefaultAsync(b => b.Id == providerId, ct);

            if (provider is null)
                return Result.Failure<bool>(ProviderErrors.NotFound(providerId));

            if (provider.Consignments.Count > 0)
                return Result.Failure<bool>(ProviderErrors.Failure(
                    "No se puede borrar una consignación que contiene productos."
                ));

            _context.Providers.Remove(provider);
            await _context.SaveChangesAsync(ct);

            await _personService.DeletePersonIfNoRolesAsync(provider.PersonId, ct);

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

    public async Task<Result<bool>> EditByIdAsync(int providerId, EditProviderDto dto, CancellationToken ct)
    {
        if (providerId <= 0)
            return Result.Failure<bool>(ProviderErrors.Failure("El Id debe ser mayor que cero."));

        var dtoValidator = new EditProviderDtoValidator();
        var dtoValidation = await dtoValidator.ValidateAsync(dto, ct);

        if (!dtoValidation.IsValid)
            return Result.Failure<bool>(ProviderErrors.Failure(dtoValidation.ToString()));

        try
        {
            var _context = await _contextFactory.CreateDbContextAsync(ct);

            var provider = await _context.Providers
                .FirstOrDefaultAsync(p => p.Id == providerId, ct);

            if (provider == null)
                return Result.Failure<bool>(ProviderErrors.NotFound(providerId));

            provider.CommissionPercent = dto.CommissionPercent;
            provider.Person.FirstName = dto.FirstName;
            provider.Person.LastName = dto.LastName;
            provider.Person.EmailAddress = dto.EmailAddress ?? provider.Person.EmailAddress;
            provider.Person.PhoneNumber = dto.PhoneNumber;
            provider.Person.Address = dto.Address;

            try
            {
                await _personService.UpdateAsync(provider.Person, ct);
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

        if (property.Equals("TotalProducts", StringComparison.OrdinalIgnoreCase))
        {
            return isDescending
            ? query.OrderByDescending(p => p.Consignments.Sum(cg => cg.Products.Count))
            : query.OrderBy(p => p.Consignments.Sum(cg => cg.Products.Count));
        }

        if (property.Equals("TotalConsignments", StringComparison.OrdinalIgnoreCase))
        {
            return isDescending
                ? query.OrderByDescending(p => p.Consignments.Count)
                : query.OrderBy(p => p.Consignments.Count);
        }

        if (sort.Property == "FullName")
        {
            return isDescending
                 ? query.OrderByDescending(p => p.Person.FirstName)
                            .ThenByDescending(p => p.Person.LastName)
                 : query.OrderBy(p => p.Person.FirstName).ThenBy(p => p.Person.LastName);
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