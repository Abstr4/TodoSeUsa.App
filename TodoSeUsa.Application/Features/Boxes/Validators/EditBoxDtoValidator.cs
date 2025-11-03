using TodoSeUsa.Application.Features.Boxes.DTOs;

namespace TodoSeUsa.Application.Features.Boxes.Validators;

public class EditBoxDtoValidator : AbstractValidator<EditBoxDto>
{
    public EditBoxDtoValidator()
    {
        RuleFor(x => x.Location)
            .MaximumLength(250).WithMessage("La longitud de la ubicación debe ser menor que 250 carácteres.");
    }
}

