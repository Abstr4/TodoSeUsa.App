using TodoSeUsa.Application.Features.Consignments.DTOs;

namespace TodoSeUsa.Application.Features.Consignments.Validators;

public class CreateConsignmentDtoValidator : AbstractValidator<CreateConsignmentDto>
{
    public CreateConsignmentDtoValidator()
    {
        RuleFor(x => x.ProviderId)
            .GreaterThan(0).WithMessage("El proveedor es obligatorio.");

        RuleFor(x => x.DateIssued)
            .NotEmpty().WithMessage("La fecha de emisión es obligatoria.")
            .LessThanOrEqualTo(DateTime.Now).WithMessage("La fecha de emisión no puede ser mayor a la fecha actual.");

        RuleFor(x => x.Notes)
            .NotEmpty().WithMessage("La ubicación es obligatoria.")
            .MaximumLength(250).WithMessage("La longitud de la ubicación debe ser menor que 250 carácteres.");
    }
}

