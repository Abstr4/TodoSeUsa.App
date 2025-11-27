namespace TodoSeUsa.Application.Features.Providers.DTOs;

public record ProviderDto
{
    public int Id { get; init; }

    public decimal CommissionPercent { get; init; }

    public int TotalConsignments { get; init; }

    public int TotalProducts { get; init; }

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public string? EmailAddress { get; init; }

    public string? PhoneNumber { get; init; }

    public string? Address { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime UpdatedAt { get; init; }

    public string FullName => $"{FirstName} {LastName}";

    public string ContactInfo => $"Email: {EmailAddress ?? "N/A"} - Número de teléfono: {PhoneNumber ?? "N/A"} - Dirección: {Address ?? "N/A"}";
}