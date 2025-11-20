namespace TodoSeUsa.Application.Features.Providers.DTOs;

public record ProviderDto
{
    public int Id { get; init; }

    public decimal CommissionPercent { get; init; }

    public int TotalConsignments { get; init; }

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public string? EmailAddress { get; init; } = string.Empty;

    public string? PhoneNumber { get; init; } = string.Empty;

    public string? Address { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; }

    public DateTime UpdatedAt { get; init; }

    public string FullName => $"{FirstName} {LastName}";

    public string ContactInfo => $"Email: {EmailAddress ?? "N/A"}, Phone: {PhoneNumber ?? "N/A"}, Address: {Address ?? "N/A"}";
}
