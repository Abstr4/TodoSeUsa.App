using System.Linq.Dynamic.Core;
using TodoSeUsa.Application.Common.Querying.CustomCases;
using TodoSeUsa.Application.Common.Services.People;
using TodoSeUsa.Application.Features.Consignors.DTOs;
using TodoSeUsa.Application.Features.Consignors.Interfaces;
using TodoSeUsa.Application.Features.Consignors.Validators;

namespace TodoSeUsa.Application.Features.Consignors.Services;

public class ConsignorService : IConsignorService
{
    private readonly ILogger<ConsignorService> _logger;
    private readonly IApplicationDbContextFactory _contextFactory;
    private readonly IPersonService _personService;

    public ConsignorService(ILogger<ConsignorService> logger, IApplicationDbContextFactory contextFactory, IPersonService personService)
    {
        _logger = logger;
        _contextFactory = contextFactory;
        _personService = personService;
    }

    public async Task<Result<PagedItems<ConsignorDto>>> GetAllAsync(QueryRequest request, CancellationToken ct)
    {
        var _context = await _contextFactory.CreateDbContextAsync(ct);

        var query = _context.Consignors
            .Include(p => p.Person)
            .Include(p => p.Consignments)
                .ThenInclude(cg => cg.Products)
            .AsNoTracking()
            .AsQueryable();

        query = QueryableExtensions.ApplyCustomFiltering(query, request.Filters, request.LogicalFilterOperator, QueryFilteringCases.ConsignorFilters);

        query = QueryableExtensions.ApplyCustomSorting(query, request.Sorts, QuerySortingCases.ConsignorSorts);

        var totalCount = await query.CountAsync(ct);

        query = query.Skip(request.Skip).Take(request.Take);

        var items = await query
            .Select(p => new ConsignorDto
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

        return Result.Success(new PagedItems<ConsignorDto>
        {
            Items = items,
            Count = totalCount
        });
    }

    public async Task<Result<int>> CreateAsync(CreateConsignorDto createConsignorDto, CancellationToken ct)
    {
        var dtoValidator = new CreateConsignorDtoValidator();
        var dtoValidationResult = await dtoValidator.ValidateAsync(createConsignorDto, ct);

        if (!dtoValidationResult.IsValid)
            return Result.Failure<int>(ConsignorErrors.Failure(dtoValidationResult.ToString()));

        try
        {
            var _context = await _contextFactory.CreateDbContextAsync(ct);

            var person = await _personService.GetByContactInfoAsync(
                createConsignorDto.EmailAddress,
                createConsignorDto.PhoneNumber,
                ct);

            if (person == null)
            {
                var newPerson = new Person
                {
                    FirstName = createConsignorDto.FirstName,
                    LastName = createConsignorDto.LastName,
                    EmailAddress = createConsignorDto.EmailAddress,
                    PhoneNumber = createConsignorDto.PhoneNumber,
                    Address = createConsignorDto.Address
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

            var existingConsignor = await _context.Consignors
                .AnyAsync(p => p.PersonId == person.Id, ct);

            if (existingConsignor)
                return Result.Failure<int>(ConsignorErrors.Failure("Esta persona ya es un consignador."));

            var consignor = new Consignor
            {
                CommissionPercent = createConsignorDto.CommissionPercent,
                PersonId = person.Id
            };

            var entry = await _context.Consignors.AddAsync(consignor, ct);
            await _context.SaveChangesAsync(ct);

            return Result.Success(entry.Entity.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to create the consignor.");
            return Result.Failure<int>(ConsignorErrors.Failure($"Ocurrió un error inesperado al intentar crear el consignador."));
        }
    }

    public async Task<Result<ConsignorDto>> GetByIdAsync(int consignorId, CancellationToken ct)
    {
        if (consignorId <= 0)
            return Result.Failure<ConsignorDto>(ConsignorErrors.Failure("El Id debe ser mayor que cero."));

        try
        {
            var _context = await _contextFactory.CreateDbContextAsync(ct);

            var consignorDto = await _context.Consignors
                .Where(p => p.Id == consignorId)
                .Select(p => new ConsignorDto
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

            if (consignorDto == null)
                return Result.Failure<ConsignorDto>(ConsignorErrors.NotFound(consignorId));

            return Result.Success(consignorDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to retrieve the consignor with ID {consignorId}.", consignorId);
            return Result.Failure<ConsignorDto>(ConsignorErrors.Failure("Ocurrió un error inesperado al intentar recuperar el consignador."));
        }
    }

    public async Task<Result> DeleteByIdAsync(int consignorId, CancellationToken ct)
    {
        if (consignorId < 1)
            return Result.Failure<bool>(ConsignorErrors.Failure("El Id debe ser mayor que cero."));

        try
        {
            var _context = await _contextFactory.CreateDbContextAsync(ct);

            var consignor = await _context.Consignors
                .Include(p => p.Consignments)
                .SingleOrDefaultAsync(b => b.Id == consignorId, ct);

            if (consignor is null)
            {
                return Result.Failure(ConsignorErrors.NotFound(consignorId));
            }

            if (consignor.Consignments.Count > 0)
            {
                return Result.Failure(ConsignorErrors.Failure(
                    "No se puede borrar un consignador que registra consignaciones."
                ));
            }

            _context.Consignors.Remove(consignor);
            await _context.SaveChangesAsync(ct);

            await _personService.DeletePersonIfNoRolesAsync(consignor.PersonId, ct);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "An error occurred while trying to delete the consignor with ID {consignorId}",
                consignorId);

            return Result.Failure(ConsignorErrors.Failure(
                "Ocurrió un error inesperado al intentar borrar el consignador."
            ));
        }
    }

    public async Task<Result> EditByIdAsync(int consignorId, EditConsignorDto dto, CancellationToken ct)
    {
        if (consignorId < 1)
            return Result.Failure(ConsignorErrors.Failure("El Id debe ser mayor que cero."));

        var dtoValidator = new EditConsignorDtoValidator();
        var dtoValidation = await dtoValidator.ValidateAsync(dto, ct);

        if (!dtoValidation.IsValid)
            return Result.Failure(ConsignorErrors.Failure(dtoValidation.ToString()));

        try
        {
            var _context = await _contextFactory.CreateDbContextAsync(ct);

            var consignor = await _context.Consignors
                .Include(p => p.Person)
                .SingleOrDefaultAsync(p => p.Id == consignorId, ct);

            if (consignor == null)
                return Result.Failure(ConsignorErrors.NotFound(consignorId));

            consignor.CommissionPercent = dto.CommissionPercent;
            consignor.Person.FirstName = dto.FirstName;
            consignor.Person.LastName = dto.LastName;
            consignor.Person.EmailAddress = dto.EmailAddress ?? consignor.Person.EmailAddress;
            consignor.Person.PhoneNumber = dto.PhoneNumber ?? consignor.Person.PhoneNumber;
            consignor.Person.Address = dto.Address ?? consignor.Person.Address;

            await _personService.UpdateAsync(consignor.Person, ct);
            var saved = await _context.SaveChangesAsync(ct) > 0;
            if (saved)
            {
                return Result.Success();
            }
            return Result.Failure(PersonErrors.Failure("No se registraron cambios en el consignador."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error editing consignor {consignorId}", consignorId);
            return Result.Failure(ConsignorErrors.Failure("Ocurrió un error inesperado al intentar actualizar al consignador."));
        }
    }
}