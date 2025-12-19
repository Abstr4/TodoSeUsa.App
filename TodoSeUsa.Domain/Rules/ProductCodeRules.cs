using System.Text.RegularExpressions;

namespace TodoSeUsa.Domain.Rules;

public static class ProductCodeRules
{
    public static string NormalizeAndValidate(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            throw new InvalidProductCodeException("El código no puede estar vacío.");

        var trimmed = raw.Trim();

        var normalized = trimmed.StartsWith("TSU-", StringComparison.OrdinalIgnoreCase)
            ? trimmed.ToUpperInvariant()
            : $"TSU-{trimmed.ToUpperInvariant()}";

        if (!Regex.IsMatch(normalized, @"^TSU-\d+$"))
            throw new InvalidProductCodeException(
                "El código es inválido, debe comenzar con 'TSU-' y continuar con números."
            );

        return normalized;
    }
}

public sealed class InvalidProductCodeException(string message) : DomainException(message) { }
