using FluentValidation;
using System.Text.RegularExpressions;

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
            .MaximumLength(100).WithMessage("La longitud máxima del apellido es de 100 carácteres");

        RuleFor(p => p.EmailAddress)
            .EmailAddress().When(p => HasValue(p.EmailAddress)).WithMessage("Email inválido.");

        RuleFor(p => p.PhoneNumber)
            .Must(IsValidE164Phone).When(p => HasValue(p.PhoneNumber))
            .WithMessage("Teléfono inválido. Use un número internacional, por ejemplo: +5491123456789.");


        RuleFor(p => p.Address)
            .MaximumLength(250).WithMessage("La longitud máxima de la dirección es de 250 carácteres")
            .When(p => HasValue(p.Address));

        RuleFor(p => p)
            .Must(HasAtLeastOneContactMethod)
            .WithMessage("Debe proporcionar al menos un teléfono, email o dirección.");
    }

    private static bool HasAtLeastOneContactMethod(Person p)
    {
        return HasValue(p.EmailAddress)
            || HasValue(p.PhoneNumber);
    }

    private static bool HasValue(string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }

    private bool IsValidE164Phone(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone)) return false;

        return Regex.IsMatch(phone, @"^\+[1-9]\d{1,14}$");
    }

}
