namespace TodoSeUsa.Application.Features.Providers.DTOs;

public record ProviderDto
{
    public int Id { get; init; }

    public decimal CommissionPercent { get; init; }

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public int TotalConsignments { get; init; }

    public DateTime CreatedAt { get; init; }
}
