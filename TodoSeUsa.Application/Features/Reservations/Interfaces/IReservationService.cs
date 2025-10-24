using TodoSeUsa.Application.Features.Reservations.DTOs;

namespace TodoSeUsa.Application.Features.Reservations.Interfaces;

public interface IReservationService
{
    Task<Result<PagedItems<ReservationDto>>> GetReservationsWithPagination(QueryItem request, CancellationToken cancellationToken);
}
