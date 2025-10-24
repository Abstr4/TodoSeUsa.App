namespace TodoSeUsa.Application.Features.Reservations;
public sealed class ReservationErrors
{
    public static Error Failure() => Error.Failure(
    "Reservations.Failure",
    "Ocurrió un error inesperado.");
}
