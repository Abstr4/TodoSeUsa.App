using TodoSeUsa.Application.Features.Products.DTOs;

namespace TodoSeUsa.Application.Features.Products.Validators;

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(p => p.Price)
            .NotEmpty().WithMessage("El precio es obligatorio.")
            .GreaterThan(0).WithMessage("El precio debe ser mayor que cero.");

        RuleFor(p => p.Quantity)
            .NotEmpty().WithMessage("La cantidad es obligatoria.")
            .GreaterThan(0).WithMessage("La cantidad debe ser mayor que cero.");

        RuleFor(p => p.Category)
            .NotEmpty().WithMessage("La categoría es obligatoria.")
            .MaximumLength(100).WithMessage("La longitud de la categoría debe ser menor que 100 carácteres.");

        RuleFor(p => p.Description)
            .NotEmpty().WithMessage("La descripción es obligatoria.")
            .MaximumLength(250).WithMessage("La longitud de la descripción debe ser menor que 250 carácteres.");

        RuleFor(p => p.Season)
            .MaximumLength(250).WithMessage("La longitud de la temporada debe ser menor que 250 carácteres.");

        RuleFor(p => p.Quality)
            .Must(q => Enum.IsDefined(q))
            .WithMessage("La calidad del producto no es válida.");

        RuleFor(p => p.RefurbishmentCost)
            .GreaterThan(0).WithMessage("El costo de arreglo debe ser mayor que cero.")
            .When(p => p.RefurbishmentCost.HasValue);

        RuleFor(p => p.ConsignmentId)
            .NotEmpty().WithMessage("El Id del consignamiento es obligatorio.")
            .GreaterThan(0).WithMessage("El Id del consignamiento debe ser mayor que cero.");

        RuleFor(p => p.BoxId)
            .GreaterThan(0).WithMessage("El Id de la caja debe ser mayor que cero.")
            .When(p => p.BoxId.HasValue);
    }
}