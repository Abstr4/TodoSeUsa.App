namespace TodoSeUsa.Application.Features.Clients.DTOs;

public record ClientDto
{
    public int Id { get; init; }

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public string? EmailAddress { get; init; }

    public string? PhoneNumber { get; init; }

    public string? Address { get; init; }

    public int TotalSales { get; init; }

    public int TotalLoanNotes { get; init; }

    public DateTime CreatedAt { get; init; }
}
