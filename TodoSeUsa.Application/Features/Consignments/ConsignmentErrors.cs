namespace TodoSeUsa.Application.Features.Consignments;
public sealed class ConsignmentErrors
{
    public static Error Failure() => Error.Failure(
    "Consignments.Failure",
    "Ocurrió un error inesperado.");
}
