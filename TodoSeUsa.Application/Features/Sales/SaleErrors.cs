namespace TodoSeUsa.Application.Features.Sales;

public sealed class SaleErrors
{
    public static Error NotFound(int saleId) => Error.NotFound(
        "Sales.NotFound",
        $"No se encontró la venta con Id = '{saleId}'.");

    public static Error SaleItemNotFound(int saleItemId) => Error.NotFound(
        "SalesItem.NotFound",
        $"No se encontró el artículo con Id = '{saleItemId}' en la venta.");

    public static Error SaleItemAlreadyReturned(int saleItemId) => Error.Failure(
        "SalesItem.AlreadyReturned",
        $"El artículo con Id = '{saleItemId}' ya ha sido devuelto.");

    public static Error Failure() => Error.Failure(
        "Sales.Failure",
        "Ocurrió un error inesperado.");

    public static Error Failure(string message) => Error.Failure(
        "Sales.Failure",
        message);
}