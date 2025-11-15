namespace TodoSeUsa.Application.Features.Consignments;
public sealed class ConsignmentErrors
{
    public static Error NotFound(int Id) => Error.NotFound(
    "Consignments.NotFound",
    $"No se encontró la consignación con Id = '{Id}'.");

    public static Error Failure() => Error.Failure(
    "Consignments.Failure",
    "Ocurrió un error inesperado.");

    public static Error Failure(string message) => Error.Failure(
    "Consignments.Failure",
    message);
}
