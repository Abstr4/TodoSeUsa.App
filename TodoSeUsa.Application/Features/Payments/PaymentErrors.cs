namespace TodoSeUsa.Application.Features.Payments;

public class PaymentErrors
{

    public static Error Failure() => Error.Failure(
    "Payments.Failure",
    "Ocurrió un error inesperado.");
}