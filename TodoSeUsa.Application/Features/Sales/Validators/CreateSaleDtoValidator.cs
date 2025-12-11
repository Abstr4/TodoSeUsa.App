using TodoSeUsa.Application.Features.Sales.DTOs;

namespace TodoSeUsa.Application.Features.Sales.Validators;

public class CreateSaleDtoValidator : AbstractValidator<CreateSaleDto>
{
    public CreateSaleDtoValidator()
    {
        RuleFor(x => x.DateIssued)
            .LessThanOrEqualTo(DateTime.Now).WithMessage("La fecha de emisión no puede ser mayor a la fecha actual.")
            .When(x => x.Notes != null);

        RuleFor(x => x.Notes)
            .MaximumLength(250).WithMessage("Las notas deben tener menos que 250 carácteres.")
            .When(x => x.Notes != null);

        RuleForEach(x => x.ProductCodes)
            .NotEmpty().WithMessage("Una venta debe contener al menos un producto.")
            .Must(code => code.StartsWith("TSU-"))
            .WithMessage("Cada código de producto debe comenzar con 'TSU-'.");

        RuleForEach(x => x.Payments)
            .SetValidator(new CreatePaymentDtoValidator())
            .When(x => x.Payments.Count > 0);
    }
}