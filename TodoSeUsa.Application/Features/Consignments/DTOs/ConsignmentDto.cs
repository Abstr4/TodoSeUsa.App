namespace TodoSeUsa.Application.Features.Consignments.DTOs;

public record ConsignmentDto
{
    public int Id { get; init; }

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public DateTime DateIssued { get; init; }

    public string Notes { get; init; } = string.Empty;

    public int ProviderId { get; init; }

    public int TotalProducts { get; init; }

    public DateTime CreatedAt { get; init; }
}
