using TodoSeUsa.Application.Features.Consignments.DTOs;

namespace TodoSeUsa.Application.Features.Consignments.Validators;

public class EditConsignmentDtoValidator : AbstractValidator<EditConsignmentDto>
{
    public EditConsignmentDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El Id del remito debe ser mayor que 0.");

        RuleFor(x => x.ConsignorId)
            .GreaterThan(0).WithMessage("El Id del consignador debe ser mayor que 0.");

        RuleFor(x => x.DateIssued)
            .LessThanOrEqualTo(DateTime.Now).WithMessage("La fecha de emisión no puede ser en el futuro.");

        RuleFor(x => x.ToString())
            .MaximumLength(250).WithMessage("La longitud de la ubicación debe ser menor que 250 carácteres.");
    }
}