using TodoSeUsa.Application.Features.Consignments.DTOs;

namespace TodoSeUsa.Application.Features.Consignments.Validators;

public class EditConsignmentDtoValidator : AbstractValidator<EditConsignmentDto>
{
    public EditConsignmentDtoValidator()
    {
        RuleFor(x => x.ToString())
            .MaximumLength(250).WithMessage("La longitud de la ubicación debe ser menor que 250 carácteres.");
    }
}