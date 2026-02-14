namespace TodoSeUsa.Application.Features.Payouts;

public static class PayoutErrors
{
    public static Error NotFound(int Id) => Error.NotFound(
    "Payouts.NotFound",
    $"La liquidación código '{Id}' no se encontró.");

    public static Error SaleItemsNotFound() => Error.NotFound(
    "Payouts.SaleItemsNotFound",
    $"No se encontraron los productos vendidos.");

    public static Error Failure() => Error.Failure(
    "Payouts.Failure",
    "Ocurrió un error inesperado.");

    public static Error Failure(string message) => Error.Failure(
    "Payouts.Failure",
    message);
}