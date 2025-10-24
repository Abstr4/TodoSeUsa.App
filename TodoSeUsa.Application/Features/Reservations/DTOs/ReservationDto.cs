using TodoSeUsa.Domain.Enums;

namespace TodoSeUsa.Application.Features.Reservations.DTOs;

public record ReservationDto
{
    public int Id { get; init; }

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public DateTime DateIssued { get; init; }

    public DateTime ExpiresAt { get; init; }

    public ReservationStatus Status { get; init; }

    public int ClientId { get; init; }

    public int TotalProducts { get; init; }

    public DateTime CreatedAt { get; init; }
}
