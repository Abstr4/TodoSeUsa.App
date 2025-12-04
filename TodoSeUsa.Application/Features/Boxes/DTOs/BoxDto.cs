namespace TodoSeUsa.Application.Features.Boxes.DTOs;

public record BoxDto
{
    public int Id { get; init; }

    public int TotalProducts { get; init; }

    public string BoxCode { get; init; } = string.Empty;

    public string Location { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; }

    public DateTime? UpdatedAt { get; init; }
}