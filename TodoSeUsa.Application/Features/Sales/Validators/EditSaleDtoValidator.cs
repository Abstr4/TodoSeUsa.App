using TodoSeUsa.Application.Features.Sales.DTOs;

namespace TodoSeUsa.Application.Features.Sales.Validators;

public class EditSaleDtoValidator : AbstractValidator<EditSaleDto>
{
    public EditSaleDtoValidator()
    {
        RuleFor(x => x.Notes)
            .MaximumLength(250).WithMessage("La longitud de la ubicación debe ser menor que 250 carácteres.")
            .When(x => x.Notes != null);
    }
}