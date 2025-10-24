namespace TodoSeUsa.Application.Features.Sales;

public class SaleErrors
{
    public static Error Failure() => Error.Failure(
    "Sales.Failure",
    "Ocurrió un error inesperado.");
}