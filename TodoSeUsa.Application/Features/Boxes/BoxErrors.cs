namespace TodoSeUsa.Application.Features.Boxes;

public static class BoxErrors
{
    public static Error NotFound(int Id) => Error.NotFound(
    "Boxes.NotFound",
    $"La caja con el Id = '{Id}' no se encontró.");

    public static Error Failure() => Error.Failure(
    "Boxes.Failure",
    "Ocurrió un error inesperado.");
}
