using TodoSeUsa.Application.Features.Boxes.DTOs;

namespace TodoSeUsa.Application.Common.Validators;

public class CreateBoxDtoValidator : AbstractValidator<CreateBoxDto>
{
    public CreateBoxDtoValidator()
    {

        RuleFor(x => x.Location)
            .NotEmpty()
            .MaximumLength(250);
    }
}

