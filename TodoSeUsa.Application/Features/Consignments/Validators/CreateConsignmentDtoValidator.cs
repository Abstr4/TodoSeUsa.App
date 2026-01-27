using TodoSeUsa.Application.Features.Consignments.DTOs;

namespace TodoSeUsa.Application.Features.Consignments.Validators;

public class CreateConsignmentDtoValidator : AbstractValidator<CreateConsignmentDto>
{
    public CreateConsignmentDtoValidator()
    {
        RuleFor(x => x.ProviderId).NotEmpty().WithMessage("El proveedor es obligatorio.")
            .GreaterThan(0).WithMessage("El código del proveedor debe ser mayor a cero.");

        RuleFor(x => x.Notes)
            .MaximumLength(250).WithMessage("Las notas deben tener menos que 250 carácteres.");
    }
}