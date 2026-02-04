using System.Linq.Dynamic.Core;
using TodoSeUsa.Application.Common.Querying.CustomCases;
using TodoSeUsa.Application.Common.Services.People;
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

        query = QueryableExtensions.ApplyCustomFiltering(query, request.Filters, request.LogicalFilterOperator, QueryFilteringCases.ProviderFilters);

        query = QueryableExtensions.ApplyCustomSorting(query, request.Sorts, QuerySortingCases.ProviderSorts);

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

    public async Task<Result> DeleteByIdAsync(int providerId, CancellationToken ct)
    {
        if (providerId < 1)
            return Result.Failure<bool>(ProviderErrors.Failure("El Id debe ser mayor que cero."));

        try
        {
            var _context = await _contextFactory.CreateDbContextAsync(ct);

            var provider = await _context.Providers
                .Include(p => p.Consignments)
                .SingleOrDefaultAsync(b => b.Id == providerId, ct);

            if (provider is null)
            {
                return Result.Failure(ProviderErrors.NotFound(providerId));
            }

            if (provider.Consignments.Count > 0)
            {
                return Result.Failure(ProviderErrors.Failure(
                    "No se puede borrar un proveedor que registra consignaciones."
                ));
            }

            _context.Providers.Remove(provider);
            await _context.SaveChangesAsync(ct);

            await _personService.DeletePersonIfNoRolesAsync(provider.PersonId, ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "An error occurred while trying to delete the provider with ID {providerId}",
                providerId);

            return Result.Failure(ProviderErrors.Failure(
                "Ocurrió un error inesperado al intentar borrar el proveedor."
            ));
        }
    }

    public async Task<Result> EditByIdAsync(int providerId, EditProviderDto dto, CancellationToken ct)
    {
        if (providerId < 1)
            return Result.Failure(ProviderErrors.Failure("El Id debe ser mayor que cero."));

        var dtoValidator = new EditProviderDtoValidator();
        var dtoValidation = await dtoValidator.ValidateAsync(dto, ct);

        if (!dtoValidation.IsValid)
            return Result.Failure(ProviderErrors.Failure(dtoValidation.ToString()));

        try
        {
            var _context = await _contextFactory.CreateDbContextAsync(ct);

            var provider = await _context.Providers
                .Include(p => p.Person)
                .SingleOrDefaultAsync(p => p.Id == providerId, ct);

            if (provider == null)
                return Result.Failure(ProviderErrors.NotFound(providerId));

            provider.CommissionPercent = dto.CommissionPercent;
            provider.Person.FirstName = dto.FirstName;
            provider.Person.LastName = dto.LastName;
            provider.Person.EmailAddress = dto.EmailAddress ?? provider.Person.EmailAddress;
            provider.Person.PhoneNumber = dto.PhoneNumber ?? provider.Person.PhoneNumber;
            provider.Person.Address = dto.Address ?? provider.Person.Address;

            await _personService.UpdateAsync(provider.Person, ct);
            var saved = await _context.SaveChangesAsync(ct) > 0;
            if (saved)
            {
                return Result.Success();
            }
            return Result.Failure(PersonErrors.Failure("No se registraron cambios en el proveedor."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error editing provider {providerId}", providerId);
            return Result.Failure(ProviderErrors.Failure("Ocurrió un error inesperado al intentar actualizar al proveedor."));
        }
    }
}