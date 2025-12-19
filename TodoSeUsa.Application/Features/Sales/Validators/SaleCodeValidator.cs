using TodoSeUsa.Domain.Constants;

namespace TodoSeUsa.Application.Features.Sales.Validators;

public class SaleCodeValidator : AbstractValidator<string>
{
    private const string _alphabet = CrockfordBase32.Alphabet;

    public SaleCodeValidator()
    {
        RuleFor(code => code)
            .NotEmpty()
                .WithMessage("El código de venta no puede estar vacío.")
            .Length(11)
                .WithMessage("El código de venta debe tener exactamente 11 caracteres.")
            .Must(HaveHyphenInMiddle)
                .WithMessage("El código de venta debe tener un guion en la posición 6.")
            .Must(UseValidAlphabet)
                .WithMessage("El código de venta contiene caracteres inválidos.");
    }

    private static bool HaveHyphenInMiddle(string code)
        => code[5] == '-';

    private static bool UseValidAlphabet(string code)
    {
        for (int i = 0; i < code.Length; i++)
        {
            if (i == 5)
                continue;

            if (!_alphabet.Contains(code[i]))
                return false;
        }

        return true;
    }
}