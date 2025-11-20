namespace TodoSeUsa.Application.Features.Providers;

public static class ProviderErrors
{
    public static Error NotFound(int Id) => Error.NotFound(
    "Providers.NotFound",
    $"El cliente con el Id = '{Id}' no se encontró.");

    public static Error Failure() => Error.Failure(
    "Providers.Failure",
    "Ocurrió un error inesperado.");
    public static Error Failure(string message) => Error.Failure(
    "Providers.Failure",
    message);
}
