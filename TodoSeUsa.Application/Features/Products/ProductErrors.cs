namespace TodoSeUsa.Application.Features.Products;

public static class ProductErrors
{
    public static Error NotFound(int Id) => Error.NotFound(
    "Products.NotFound",
    $"El producto con el Id = '{Id}' no se encontró.");

    public static Error Failure() => Error.Failure(
    "Products.Failure",
    "Ocurrió un error inesperado.");

    public static Error Failure(string message) => Error.Failure(
    "Products.Failure",
    message);
}
