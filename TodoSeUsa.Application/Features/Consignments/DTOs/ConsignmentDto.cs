namespace TodoSeUsa.Application.Features.Consignments.DTOs;

public record ConsignmentDto
{
    public int Id { get; init; }

    public string ProviderFirstName { get; init; } = string.Empty;

    public string ProviderLastName { get; init; } = string.Empty;

    public int ProviderId { get; init; }

    public string? Notes { get; init; }

    public int TotalProducts { get; init; }

    public DateTime DateIssued { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime UpdatedAt { get; init; }

    public string ProviderFullName => $"{ProviderFirstName} {ProviderLastName}";
}