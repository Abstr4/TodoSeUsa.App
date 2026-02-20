using TodoSeUsa.Application.Features.Products.DTOs;

namespace TodoSeUsa.Application.Features.Products.Validators;

public class EditProductDtoValidator : AbstractValidator<EditProductDto>
{
    public EditProductDtoValidator()
    {
        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("El precio debe ser mayor que 0.");

        RuleFor(x => x.Category)
            .MaximumLength(250).WithMessage("La longitud de la ubicación debe ser menor que 250 carácteres.");

        RuleFor(x => x.Description)
            .MaximumLength(250).WithMessage("La longitud de la descripción debe ser menor que 250 carácteres.");

        RuleFor(x => x.Size)
            .MaximumLength(250).WithMessage("La longitud del tamaño debe ser menor que 250 carácteres.");

        RuleFor(x => x.Season).MaximumLength(250)
            .WithMessage("La longitud de la temporada debe ser menor que 250 carácteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Season));

        RuleFor(x => x.Brand).MaximumLength(250)
            .WithMessage("La longitud de la marca debe ser menor que 250 carácteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Brand));

        RuleFor(x => x.RefurbishmentCost)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El costo de reacondicionamiento debe ser mayor o igual a 0.")
            .When(x => x.RefurbishmentCost != null);
    }
}