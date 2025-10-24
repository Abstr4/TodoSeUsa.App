namespace TodoSeUsa.Application.Features.Clients;

public static class ClientErrors
{
    public static Error NotFound(int Id) => Error.NotFound(
    "Clients.NotFound",
    $"El cliente con el Id = '{Id}' no se encontró.");

    public static Error Failure() => Error.Failure(
    "Clients.Failure",
    "Ocurrió un error inesperado.");
}
