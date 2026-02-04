using TodoSeUsa.Application.Features.Payments.DTOs;

namespace TodoSeUsa.Application.Features.Sales.Validators;

public class CreatePaymentDtoValidator : AbstractValidator<CreatePaymentDto>
{
    public CreatePaymentDtoValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0)
                .WithMessage("El monto debe ser mayor que cero.");

        RuleFor(x => x.Method)
            .IsInEnum()
                .WithMessage("Método de pago inválido.");

        RuleFor(x => x.Date)
            .LessThanOrEqualTo(DateTime.Now)
                .WithMessage("La fecha debe estar en el pasado.")
            .When(x => x.Date.HasValue);

        RuleFor(x => x.Notes)
            .MaximumLength(250)
                .WithMessage("Las notas no pueden superar los 250 caracteres.")
            .When(x => x.Notes != null);
    }
}