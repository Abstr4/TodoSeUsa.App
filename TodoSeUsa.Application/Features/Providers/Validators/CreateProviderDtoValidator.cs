using TodoSeUsa.Application.Features.Providers.DTOs;

namespace TodoSeUsa.Application.Features.Providers.Validators;

public class CreateProviderDtoValidator : AbstractValidator<CreateProviderDto>
{
    public CreateProviderDtoValidator()
    {
        RuleFor(p => p.CommissionPercent)
            .GreaterThanOrEqualTo(0).WithMessage("El porcentaje de comisión debe ser mayor o igual a 0.")
            .LessThanOrEqualTo(100).WithMessage("El porcentaje de comisión debe ser menor o igual a 100.");
    }
}
