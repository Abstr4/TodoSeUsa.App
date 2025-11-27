using TodoSeUsa.Application.Features.Boxes.DTOs;

namespace TodoSeUsa.Application.Features.Boxes.Validators;

public class CreateBoxDtoValidator : AbstractValidator<CreateBoxDto>
{
    public CreateBoxDtoValidator()
    {
        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("La ubicación es obligatoria.")
            .MaximumLength(250).WithMessage("La longitud de la ubicación debe ser menor que 250 carácteres.");
    }
}