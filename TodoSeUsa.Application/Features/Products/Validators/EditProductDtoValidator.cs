using TodoSeUsa.Application.Features.Products.DTOs;

namespace TodoSeUsa.Application.Features.Products.Validators;

public class EditProductDtoValidator : AbstractValidator<EditProductDto>
{
    public EditProductDtoValidator()
    {
        RuleFor(x => x.Category)
            .MaximumLength(250).WithMessage("La longitud de la ubicación debe ser menor que 250 carácteres.");
    }
}