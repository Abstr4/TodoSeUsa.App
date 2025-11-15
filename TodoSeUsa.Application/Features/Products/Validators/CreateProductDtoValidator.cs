using TodoSeUsa.Application.Features.Products.DTOs;
using TodoSeUsa.Domain.Enums;

namespace TodoSeUsa.Application.Features.Products.Validators;

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(x => x.Price)
            .NotEmpty().WithMessage("El precio es obligatorio.")
            .GreaterThan(0).WithMessage("El precio debe ser mayor que cero.");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("La categoría es obligatoria.")
            .MaximumLength(100).WithMessage("La longitud de la categoría debe ser menor que 100 carácteres.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("La descripción es obligatoria.")
            .MaximumLength(250).WithMessage("La longitud de la descripción debe ser menor que 100 carácteres.");

        RuleFor(x => x.Quality)
            .Must(q => Enum.IsDefined(q))
            .WithMessage("La calidad del producto no es válida.");

        RuleFor(x => x.Season)
           .IsInEnum().WithMessage("La temporada del producto no es válida.")
           .When(x => x.Season.HasValue);

        RuleFor(x => x.RefurbishmentCost)
            .GreaterThan(0).WithMessage("El costo de arreglo debe ser mayor que cero.")
            .When(x => x.RefurbishmentCost.HasValue);

        RuleFor(x => x.ConsignmentId)
            .NotEmpty().WithMessage("El Id del consignamiento es obligatorio.")
            .GreaterThan(0).WithMessage("El Id del consignamiento debe ser mayor que cero.");

        RuleFor(x => x.BoxId)
            .GreaterThan(0).WithMessage("El Id de la caja debe ser mayor que cero.")
            .When(x => x.BoxId.HasValue);

    }
}

