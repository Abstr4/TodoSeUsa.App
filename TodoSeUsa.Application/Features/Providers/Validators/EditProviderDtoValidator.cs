using FluentValidation;
using System.Text.RegularExpressions;
using TodoSeUsa.Application.Features.Providers.DTOs;
using TodoSeUsa.Domain.Validators;

namespace TodoSeUsa.Application.Features.Providers.Validators;

public class EditProviderDtoValidator : AbstractValidator<EditProviderDto>
{
    public EditProviderDtoValidator()
    {
        RuleFor(p => p.CommissionPercent)
            .GreaterThanOrEqualTo(0).WithMessage("El porcentaje de comisión debe ser mayor o igual a 0.")
            .LessThanOrEqualTo(100).WithMessage("El porcentaje de comisión debe ser menor o igual a 100.");

        RuleFor(p => p.FirstName)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(50).WithMessage("La longitud máxima del nombre es de 50 carácteres.");

        RuleFor(p => p.LastName)
            .NotEmpty().WithMessage("El apellido es obligatorio.")
            .MaximumLength(100).WithMessage("La longitud máxima del apellido es de 100 carácteres.");

        RuleFor(p => p.EmailAddress)
            .Must(email => DomainValidators.EmailValidator(email!))
            .When(p => DomainValidators.HasValue(p.EmailAddress)).WithMessage("Email inválido.");

        RuleFor(p => p.PhoneNumber)
            .Must(phone => DomainValidators.PhoneValidator(phone!))
            .When(p => DomainValidators.HasValue(p.PhoneNumber))
            .WithMessage("Teléfono inválido. Use un número internacional, por ejemplo: +5491123456789.");

        RuleFor(p => p.Address)
            .MaximumLength(250).WithMessage("La longitud máxima de la dirección es de 250 carácteres.")
            .When(p => DomainValidators.HasValue(p.Address));

        RuleFor(p => p)
            .Must(p => DomainValidators.HasValue(p.EmailAddress) || DomainValidators.HasValue(p.PhoneNumber))
            .WithMessage("Debe proporcionar un email o número de teléfono.");
    }
}
