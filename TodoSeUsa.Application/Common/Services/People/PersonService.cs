namespace TodoSeUsa.Application.Common.Services.People;

internal class PersonService : IPersonService
{
    private readonly IApplicationDbContextFactory _contextFactory;

    public PersonService(IApplicationDbContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<Person?> GetByContactInfoAsync(
        string? email,
        string? phoneNumber,
        CancellationToken ct)
    {
        if (email == null && phoneNumber == null)
            return null;

        var _context = await _contextFactory.CreateDbContextAsync(ct);

        return await _context.Persons
            .Include(p => p.Client)
            .Include(p => p.Provider)
            .FirstOrDefaultAsync(p =>
                email != null && p.EmailAddress == email ||
                phoneNumber != null && p.PhoneNumber == phoneNumber,
                ct);
    }

    public async Task<Person> CreateAsync(Person person, CancellationToken ct)
    {
        var validator = new PersonValidator();
        var validation = await validator.ValidateAsync(person, ct);

        if (!validation.IsValid)
            throw new ValidationException(validation.ToString());

        var _context = await _contextFactory.CreateDbContextAsync(ct);

        _context.Persons.Add(person);
        await _context.SaveChangesAsync(ct);

        return person;
    }

    public async Task DeletePersonIfNoRolesAsync(int personId, CancellationToken ct)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(personId);

        var _context = await _contextFactory.CreateDbContextAsync(ct);

        var person = await _context.Persons
            .Include(p => p.Client)
            .Include(p => p.Provider)
            .FirstOrDefaultAsync(p => p.Id == personId, ct);

        if (person == null)
            return;

        if (person.Client == null && person.Provider == null)
        {
            _context.Persons.Remove(person);
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task UpdateAsync(Person person, CancellationToken ct)
    {
        var validator = new PersonValidator();
        var result = await validator.ValidateAsync(person, ct);

        if (!result.IsValid)
            throw new ValidationException(result.ToString());

        var _context = await _contextFactory.CreateDbContextAsync(ct);

        if (!string.IsNullOrEmpty(person.EmailAddress))
        {
            bool emailExists = await _context.Persons
                .AnyAsync(p => p.Id != person.Id && p.EmailAddress == person.EmailAddress, ct);

            if (emailExists)
                throw new ValidationException("Ya existe una persona con ese correo electrónico.");
        }

        if (!string.IsNullOrEmpty(person.PhoneNumber))
        {
            bool phoneExists = await _context.Persons
                .AnyAsync(p => p.Id != person.Id && p.PhoneNumber == person.PhoneNumber, ct);

            if (phoneExists)
                throw new ValidationException("Ya existe una persona con ese número de teléfono.");
        }

        await _context.SaveChangesAsync(ct);
    }
}