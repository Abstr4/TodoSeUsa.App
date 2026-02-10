namespace TodoSeUsa.Application.Features.Consignments.DTOs;

public record ConsignmentDto
{
    public int Id { get; init; }

    public string Code { get; init; } = string.Empty;

    public string ConsignorFirstName { get; init; } = string.Empty;

    public string ConsignorLastName { get; init; } = string.Empty;

    public int ConsignorId { get; init; }

    public string? Notes { get; init; }

    public int TotalProducts { get; init; }

    public DateTime DateIssued { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime? UpdatedAt { get; init; }

    public string ConsignorFullName => $"{ConsignorFirstName} {ConsignorLastName}";
}