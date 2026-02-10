namespace TodoSeUsa.Application.Features.Consignors;

public static class ConsignorErrors
{
    public static Error NotFound(int Id) => Error.NotFound(
    "Consignors.NotFound",
    $"El cliente con el Id = '{Id}' no se encontró.");

    public static Error Failure() => Error.Failure(
    "Consignors.Failure",
    "Ocurrió un error inesperado.");

    public static Error Failure(string message) => Error.Failure(
    "Consignors.Failure",
    message);
}