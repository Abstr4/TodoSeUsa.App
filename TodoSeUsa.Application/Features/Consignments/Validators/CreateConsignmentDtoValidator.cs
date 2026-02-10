using TodoSeUsa.Application.Features.Consignments.DTOs;

namespace TodoSeUsa.Application.Features.Consignments.Validators;

public class CreateConsignmentDtoValidator : AbstractValidator<CreateConsignmentDto>
{
    public CreateConsignmentDtoValidator()
    {
        RuleFor(x => x.ConsignorId).NotEmpty().WithMessage("El consignador es obligatorio.")
            .GreaterThan(0).WithMessage("El código del consignador debe ser mayor a cero.");

        RuleFor(x => x.Notes)
            .MaximumLength(250).WithMessage("Las notas deben tener menos que 250 carácteres.");
    }
}