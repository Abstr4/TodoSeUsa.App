using System.Text.RegularExpressions;
using TodoSeUsa.Domain.Validators;

namespace TodoSeUsa.Application.Common.Validators;

public class PersonValidator : AbstractValidator<Person>
{
    public PersonValidator()
    {
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
            .Must(DomainValidators.HasAtLeastOneContactMethod)
            .WithMessage("Debe proporcionar un email o número de teléfono.");
    }
}
