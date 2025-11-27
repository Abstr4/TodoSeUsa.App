namespace TodoSeUsa.Application.Common.Interfaces;

public interface IPersonService
{
    Task<Person?> GetByContactInfoAsync(string? email, string? phoneNumber, CancellationToken ct);

    Task<Person> CreateAsync(Person person, CancellationToken ct);

    Task DeletePersonIfNoRolesAsync(int personId, CancellationToken ct);

    Task UpdateAsync(Person person, CancellationToken ct);
}