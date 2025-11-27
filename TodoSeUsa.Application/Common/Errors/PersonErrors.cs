namespace TodoSeUsa.Application.Common.Errors;

public static class PersonErrors
{
    public static Error NotFound(int Id) => Error.NotFound(
    "Persons.NotFound",
    $"No se encontró la persona con el Id = '{Id}'.");

    public static Error Failure() => Error.Failure(
    "Persons.Failure",
    "Ocurrió un error inesperado.");

    public static Error Failure(string message) => Error.Failure(
    "Persons.Failure",
    message);
}