using TodoSeUsa.Application.Common.Validators;

namespace TodoSeUsa.Application.Common.Services;

internal class PersonService : IPersonService
{
    private readonly IApplicationDbContext _context;

    public PersonService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Person?> GetByContactInfoAsync(
        string? email,
        string? phoneNumber,
        CancellationToken cancellationToken)
    {
        if (email == null && phoneNumber == null)
            return null;

        return await _context.Persons
            .Include(p => p.Client)
            .Include(p => p.Provider)
            .FirstOrDefaultAsync(p =>
                (email != null && p.EmailAddress == email) ||
                (phoneNumber != null && p.PhoneNumber == phoneNumber),
                cancellationToken);
    }

    public async Task<Person> CreateAsync(Person person, CancellationToken cancellationToken)
    {
        var validator = new PersonValidator();
        var validation = await validator.ValidateAsync(person, cancellationToken);

        if (!validation.IsValid)
            throw new ValidationException(validation.ToString());

        _context.Persons.Add(person);
        await _context.SaveChangesAsync(cancellationToken);

        return person;
    }

    public async Task DeletePersonIfNoRolesAsync(int personId, CancellationToken cancellationToken)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(personId);

        var person = await _context.Persons
            .Include(p => p.Client)
            .Include(p => p.Provider)
            .FirstOrDefaultAsync(p => p.Id == personId, cancellationToken);

        if (person == null)
            return;

        if (person.Client == null && person.Provider == null)
        {
            _context.Persons.Remove(person);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task UpdateAsync(Person person, CancellationToken cancellationToken)
    {
        var validator = new PersonValidator();
        var result = await validator.ValidateAsync(person, cancellationToken);

        if (!result.IsValid)
            throw new ValidationException(result.ToString());

        await _context.SaveChangesAsync(cancellationToken);
    }
}
